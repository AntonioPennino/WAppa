// Controllers/FavoriteLocationsController.cs
using Microsoft.AspNetCore.Authorization; // Per [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Per accedere ai claims dell'utente (es. ID)
using System.Threading.Tasks;
using WeatherApp.Data;
using WeatherApp.Dtos.FavoriteLocation;
using WeatherApp.Dtos.Weather; // Per WeatherForecastDto
using WeatherApp.Models;
using WeatherApp.Services; // Per IGeocodingService e IWeatherService

namespace WeatherApp.Controllers
{
    [Authorize] // Richiede l'autenticazione per tutti gli endpoint in questo controller
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteLocationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeocodingService _geocodingService;
        private readonly IWeatherService _weatherService;
        private readonly ILogger<FavoriteLocationsController> _logger;

        public FavoriteLocationsController(
            ApplicationDbContext context,
            IGeocodingService geocodingService,
            IWeatherService weatherService,
            ILogger<FavoriteLocationsController> logger)
        {
            _context = context;
            _geocodingService = geocodingService;
            _weatherService = weatherService;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogError("User ID claim (NameIdentifier) not found or invalid in token for user: {User}", User.Identity?.Name ?? "Unknown");
                throw new UnauthorizedAccessException("User ID could not be determined from token.");
            }
            return userId;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FavoriteLocationResponseDto>>> AddFavoriteLocation(AddFavoriteLocationDto addLocationDto)
        {
            var serviceResponse = new ServiceResponse<FavoriteLocationResponseDto>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("AddFavoriteLocation: Unauthorized access attempt. Message: {Message}", ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("User {UserId} attempting to add favorite location: {LocationName}", userId, addLocationDto.LocationName);
            var geocodeResponse = await _geocodingService.GetCoordinatesAsync(addLocationDto.LocationName);
            if (!geocodeResponse.Success || geocodeResponse.Data == null)
            {
                _logger.LogWarning("User {UserId} - Geocoding failed for {LocationName}: {GeoMessage}", userId, addLocationDto.LocationName, geocodeResponse.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = geocodeResponse.Message ?? $"Could not find coordinates for '{addLocationDto.LocationName}'.";
                return NotFound(serviceResponse);
            }

            var geoData = geocodeResponse.Data;
            _logger.LogInformation("User {UserId} - Geocoding for {LocationName} successful: {GeoName}, Lat={Lat}, Lon={Lon}", userId, addLocationDto.LocationName, geoData.Name, geoData.Latitude, geoData.Longitude);

            var existingLocation = await _context.FavoriteLocations
                .FirstOrDefaultAsync(fl => fl.UserId == userId && fl.Latitude == geoData.Latitude && fl.Longitude == geoData.Longitude);

            if (existingLocation != null)
            {
                _logger.LogInformation("User {UserId} - Location '{GeoName}' already exists in favorites.", userId, geoData.Name);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Location '{geoData.Name}' is already in your favorites.";
                return Conflict(serviceResponse);
            }

            var newFavoriteLocation = new FavoriteLocation
            {
                Name = geoData.Name,
                Latitude = geoData.Latitude,
                Longitude = geoData.Longitude,
                UserId = userId
            };

            _context.FavoriteLocations.Add(newFavoriteLocation);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {UserId} - Saved new favorite location ID {FavId} for '{Name}'.", userId, newFavoriteLocation.Id, newFavoriteLocation.Name);

            // Recupera i dati meteo per la nuova località, PASSANDO IL NOME
            var weatherResponse = await _weatherService.GetWeatherDataAsync(newFavoriteLocation.Latitude, newFavoriteLocation.Longitude, newFavoriteLocation.Name);
            _logger.LogInformation("User {UserId} - Weather data fetched for new favorite {Name}: Success={WeatherSuccess}", userId, newFavoriteLocation.Name, weatherResponse.Success);


            serviceResponse.Data = new FavoriteLocationResponseDto
            {
                Id = newFavoriteLocation.Id,
                Name = newFavoriteLocation.Name,
                Latitude = newFavoriteLocation.Latitude,
                Longitude = newFavoriteLocation.Longitude,
                WeatherData = weatherResponse.Success ? weatherResponse.Data : null
            };
            serviceResponse.Message = $"Location '{newFavoriteLocation.Name}' added to favorites.";
            
            return CreatedAtAction(nameof(GetFavoriteLocationById), new { id = newFavoriteLocation.Id }, serviceResponse);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<FavoriteLocationResponseDto>>>> GetFavoriteLocations()
        {
            var serviceResponse = new ServiceResponse<List<FavoriteLocationResponseDto>>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("GetFavoriteLocations: Unauthorized access attempt. Message: {Message}", ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("User {UserId} fetching favorite locations.", userId);
            var favoriteLocations = await _context.FavoriteLocations
                                        .Where(fl => fl.UserId == userId)
                                        .ToListAsync();

            var responseDtos = new List<FavoriteLocationResponseDto>();
            foreach (var location in favoriteLocations)
            {
                // Recupera i dati meteo, PASSANDO IL NOME DELLA LOCALITÀ SALVATA
                var weatherResponse = await _weatherService.GetWeatherDataAsync(location.Latitude, location.Longitude, location.Name);
                if (!weatherResponse.Success)
                {
                     _logger.LogWarning("User {UserId} - Failed to get weather for favorite location '{LocationName}' (ID: {LocationId}). Message: {WeatherMessage}", userId, location.Name, location.Id, weatherResponse.Message);
                }
                responseDtos.Add(new FavoriteLocationResponseDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    WeatherData = weatherResponse.Success ? weatherResponse.Data : null
                });
            }

            serviceResponse.Data = responseDtos;
            serviceResponse.Message = responseDtos.Any() ? "Favorite locations retrieved." : "No favorite locations found for user.";
             _logger.LogInformation("User {UserId} - Retrieved {Count} favorite locations.", userId, responseDtos.Count);
            return Ok(serviceResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<FavoriteLocationResponseDto>>> GetFavoriteLocationById(int id)
        {
            var serviceResponse = new ServiceResponse<FavoriteLocationResponseDto>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                 _logger.LogWarning("GetFavoriteLocationById: Unauthorized access attempt for ID {LocationId}. Message: {Message}", id, ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("User {UserId} fetching favorite location by ID {LocationId}.", userId, id);
            var favoriteLocation = await _context.FavoriteLocations.FirstOrDefaultAsync(fl => fl.Id == id && fl.UserId == userId);

            if (favoriteLocation == null)
            {
                _logger.LogWarning("User {UserId} - Favorite location ID {LocationId} not found or access denied.", userId, id);
                serviceResponse.Success = false;
                serviceResponse.Message = "Favorite location not found or access denied.";
                return NotFound(serviceResponse);
            }
            
            // Recupera i dati meteo, PASSANDO IL NOME DELLA LOCALITÀ SALVATA
            var weatherResponse = await _weatherService.GetWeatherDataAsync(favoriteLocation.Latitude, favoriteLocation.Longitude, favoriteLocation.Name);
            if (!weatherResponse.Success)
            {
                _logger.LogWarning("User {UserId} - Failed to get weather for favorite location '{LocationName}' (ID: {LocationId}) in GetById. Message: {WeatherMessage}", userId, favoriteLocation.Name, favoriteLocation.Id, weatherResponse.Message);
            }

            serviceResponse.Data = new FavoriteLocationResponseDto
            {
                Id = favoriteLocation.Id,
                Name = favoriteLocation.Name,
                Latitude = favoriteLocation.Latitude,
                Longitude = favoriteLocation.Longitude,
                WeatherData = weatherResponse.Success ? weatherResponse.Data : null
            };
            serviceResponse.Message = "Favorite location retrieved.";
            return Ok(serviceResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteFavoriteLocation(int id)
        {
            var serviceResponse = new ServiceResponse<string>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("DeleteFavoriteLocation: Unauthorized access attempt for ID {LocationId}. Message: {Message}", id, ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("User {UserId} attempting to delete favorite location ID {LocationId}.", userId, id);
            var favoriteLocation = await _context.FavoriteLocations.FirstOrDefaultAsync(fl => fl.Id == id && fl.UserId == userId);

            if (favoriteLocation == null)
            {
                _logger.LogWarning("User {UserId} - Favorite location ID {LocationId} for deletion not found or access denied.", userId, id);
                serviceResponse.Success = false;
                serviceResponse.Message = "Favorite location not found or you do not have permission to delete it.";
                return NotFound(serviceResponse);
            }

            var locationName = favoriteLocation.Name; // Salva il nome per il log
            _context.FavoriteLocations.Remove(favoriteLocation);
            await _context.SaveChangesAsync();

            serviceResponse.Message = $"Favorite location '{locationName}' (ID: {id}) deleted successfully.";
            _logger.LogInformation("User {UserId} - Successfully deleted favorite location '{LocationName}' (ID: {LocationId}).", userId, locationName, id);
            return Ok(serviceResponse);
        }
    }
}

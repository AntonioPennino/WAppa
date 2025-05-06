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
        private readonly ILogger<FavoriteLocationsController> _logger; // Per il logging

        public FavoriteLocationsController(
            ApplicationDbContext context,
            IGeocodingService geocodingService,
            IWeatherService weatherService,
            ILogger<FavoriteLocationsController> logger) // Aggiunto logger
        {
            _context = context;
            _geocodingService = geocodingService;
            _weatherService = weatherService;
            _logger = logger;
        }

        // Metodo helper per ottenere l'ID dell'utente corrente
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                // Questo non dovrebbe accadere se [Authorize] funziona correttamente
                // e il token contiene l'ID utente come NameIdentifier.
                _logger.LogError("User ID claim (NameIdentifier) not found or invalid in token.");
                throw new UnauthorizedAccessException("User ID could not be determined from token.");
            }
            return userId;
        }

        // POST: api/favoritelocations
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
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            // 1. Usa il GeocodingService per ottenere le coordinate
            var geocodeResponse = await _geocodingService.GetCoordinatesAsync(addLocationDto.LocationName);
            if (!geocodeResponse.Success || geocodeResponse.Data == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = geocodeResponse.Message ?? $"Could not find coordinates for '{addLocationDto.LocationName}'.";
                return NotFound(serviceResponse);
            }

            var geoData = geocodeResponse.Data;

            // 2. Controlla se l'utente ha già questa località (opzionale, basato su coordinate)
            // Questo previene duplicati esatti per lo stesso utente
            var existingLocation = await _context.FavoriteLocations
                .FirstOrDefaultAsync(fl => fl.UserId == userId && fl.Latitude == geoData.Latitude && fl.Longitude == geoData.Longitude);

            if (existingLocation != null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Location '{geoData.Name}' is already in your favorites.";
                return Conflict(serviceResponse); // 409 Conflict è appropriato per duplicati
            }

            // 3. Crea e salva la nuova FavoriteLocation
            var newFavoriteLocation = new FavoriteLocation
            {
                Name = geoData.Name, // Usa il nome restituito dal geocoding per coerenza
                Latitude = geoData.Latitude,
                Longitude = geoData.Longitude,
                UserId = userId
            };

            _context.FavoriteLocations.Add(newFavoriteLocation);
            await _context.SaveChangesAsync();

            // 4. (Opzionale) Recupera i dati meteo per la nuova località
            var weatherResponse = await _weatherService.GetWeatherDataAsync(newFavoriteLocation.Latitude, newFavoriteLocation.Longitude);

            serviceResponse.Data = new FavoriteLocationResponseDto
            {
                Id = newFavoriteLocation.Id,
                Name = newFavoriteLocation.Name,
                Latitude = newFavoriteLocation.Latitude,
                Longitude = newFavoriteLocation.Longitude,
                WeatherData = weatherResponse.Success ? weatherResponse.Data : null // Includi dati meteo se disponibili
            };
            serviceResponse.Message = $"Location '{newFavoriteLocation.Name}' added to favorites.";
            
            return CreatedAtAction(nameof(GetFavoriteLocationById), new { id = newFavoriteLocation.Id }, serviceResponse);
        }

        // GET: api/favoritelocations
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
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            var favoriteLocations = await _context.FavoriteLocations
                                        .Where(fl => fl.UserId == userId)
                                        .ToListAsync();

            var responseDtos = new List<FavoriteLocationResponseDto>();
            foreach (var location in favoriteLocations)
            {
                var weatherResponse = await _weatherService.GetWeatherDataAsync(location.Latitude, location.Longitude);
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
            serviceResponse.Message = responseDtos.Any() ? "Favorite locations retrieved." : "No favorite locations found.";
            return Ok(serviceResponse);
        }

        // GET: api/favoritelocations/{id}
        // Utile per il CreatedAtAction e per recuperare una singola località se necessario
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
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            var favoriteLocation = await _context.FavoriteLocations.FirstOrDefaultAsync(fl => fl.Id == id && fl.UserId == userId);

            if (favoriteLocation == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Favorite location not found or access denied.";
                return NotFound(serviceResponse);
            }
            
            var weatherResponse = await _weatherService.GetWeatherDataAsync(favoriteLocation.Latitude, favoriteLocation.Longitude);

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


        // DELETE: api/favoritelocations/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteFavoriteLocation(int id)
        {
            var serviceResponse = new ServiceResponse<string>(); // string per un semplice messaggio
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            var favoriteLocation = await _context.FavoriteLocations.FirstOrDefaultAsync(fl => fl.Id == id && fl.UserId == userId);

            if (favoriteLocation == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Favorite location not found or you do not have permission to delete it.";
                return NotFound(serviceResponse);
            }

            _context.FavoriteLocations.Remove(favoriteLocation);
            await _context.SaveChangesAsync();

            serviceResponse.Message = $"Favorite location '{favoriteLocation.Name}' (ID: {id}) deleted successfully.";
            // Non c'è Data qui, quindi possiamo ometterlo o impostare Data = null;
            return Ok(serviceResponse);
        }
    }
}

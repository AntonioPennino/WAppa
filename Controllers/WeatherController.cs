// Controllers/WeatherController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeatherApp.Dtos.Weather; // Per WeatherForecastDto
using WeatherApp.Services;    // Per IWeatherService e IGeocodingService

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IGeocodingService _geocodingService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(
            IWeatherService weatherService,
            IGeocodingService geocodingService,
            ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _geocodingService = geocodingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<WeatherForecastDto>>> GetWeather(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] string? query)
        {
            var response = new ServiceResponse<WeatherForecastDto>();
            _logger.LogInformation("WeatherController: Received request with Lat={Lat}, Lon={Lon}, Query='{Query}'", latitude, longitude, query);


            if (latitude.HasValue && longitude.HasValue)
            {
                _logger.LogInformation("WeatherController: Using provided coordinates Lat={Lat}, Lon={Lon} to fetch weather.", latitude.Value, longitude.Value);
                // Quando si usano lat/lon diretti, non abbiamo un nome geocodificato, quindi locationNameFromGeocoding sarà null.
                // Il WeatherService userà il fallback (es. timezone) per LocationName.
                // Oppure, potremmo provare a fare un "reverse geocoding" qui se volessimo un nome, ma aumenterebbe la complessità.
                // O potremmo passare la query originale se fosse in qualche modo rilevante (ma qui non c'è query).
                response = await _weatherService.GetWeatherDataAsync(latitude.Value, longitude.Value, locationNameFromGeocoding: query); // Passiamo query se presente, altrimenti null
            }
            else if (!string.IsNullOrWhiteSpace(query))
            {
                _logger.LogInformation("WeatherController: Geocoding query '{Query}'", query);
                var geocodeResponse = await _geocodingService.GetCoordinatesAsync(query);

                if (!geocodeResponse.Success || geocodeResponse.Data == null)
                {
                    _logger.LogWarning("WeatherController: Geocoding failed or no data for query '{Query}'. Message: {Msg}", query, geocodeResponse.Message);
                    response.Success = false;
                    response.Message = geocodeResponse.Message ?? $"Could not find coordinates for '{query}'.";
                    return NotFound(response);
                }

                var geoData = geocodeResponse.Data;
                _logger.LogInformation("WeatherController: Geocoding for '{Query}' successful. Result: Name='{Name}', Lat={Lat}, Lon={Lon}", query, geoData.Name, geoData.Latitude, geoData.Longitude);

                _logger.LogInformation("WeatherController: Fetching weather for geocoded coordinates Lat={Lat}, Lon={Lon} with LocationName='{LocationName}'", geoData.Latitude, geoData.Longitude, geoData.Name);
                // PASSA geoData.Name (il nome dalla geocodifica) a GetWeatherDataAsync
                response = await _weatherService.GetWeatherDataAsync(geoData.Latitude, geoData.Longitude, geoData.Name); 
            }
            else
            {
                _logger.LogWarning("WeatherController: Invalid request parameters. Latitude/Longitude or Query must be provided.");
                response.Success = false;
                response.Message = "Please provide either 'latitude' and 'longitude' or a 'query' parameter.";
                return BadRequest(response);
            }

            if (!response.Success)
            {
                _logger.LogWarning("WeatherController: WeatherService call failed or returned no data for Lat={Lat}, Lon={Lon}, Query='{Query}'. Message: {Message}", latitude, longitude, query, response.Message);
                // Determina lo status code in base al messaggio o a un campo specifico di errore se disponibile
                return StatusCode(response.Data == null && response.Message.Contains("Could not find coordinates") ? 404 : 500, response); 
            }
            
            _logger.LogInformation("WeatherController: Successfully retrieved weather data for Lat={Lat}, Lon={Lon}, Query='{Query}'", latitude, longitude, query);
            return Ok(response);
        }
    }
}

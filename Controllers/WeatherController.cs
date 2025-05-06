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

        // Endpoint per ottenere dati meteo
        // Esempi di chiamata:
        // GET /api/weather?latitude=41.90&longitude=12.49
        // GET /api/weather?query=Roma
        // GET /api/weather?query=90210
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<WeatherForecastDto>>> GetWeather(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] string? query)
        {
            var response = new ServiceResponse<WeatherForecastDto>();

            if (latitude.HasValue && longitude.HasValue)
            {
                // Caso 1: Latitudine e Longitudine fornite
                _logger.LogInformation("Fetching weather for Latitude: {Latitude}, Longitude: {Longitude}", latitude.Value, longitude.Value);
                response = await _weatherService.GetWeatherDataAsync(latitude.Value, longitude.Value);
            }
            else if (!string.IsNullOrWhiteSpace(query))
            {
                // Caso 2: Query (nome città/CAP) fornita
                _logger.LogInformation("Fetching weather for query: {Query}", query);
                var geocodeResponse = await _geocodingService.GetCoordinatesAsync(query);

                if (!geocodeResponse.Success || geocodeResponse.Data == null)
                {
                    response.Success = false;
                    response.Message = geocodeResponse.Message ?? $"Could not find coordinates for '{query}'.";
                    return NotFound(response);
                }

                var geoData = geocodeResponse.Data;
                _logger.LogInformation("Geocoded '{Query}' to Latitude: {Latitude}, Longitude: {Longitude}", query, geoData.Latitude, geoData.Longitude);
                response = await _weatherService.GetWeatherDataAsync(geoData.Latitude, geoData.Longitude);
            }
            else
            {
                // Caso 3: Input non valido
                response.Success = false;
                response.Message = "Please provide either 'latitude' and 'longitude' or a 'query' parameter.";
                return BadRequest(response);
            }

            if (!response.Success)
            {
                // Se il WeatherService fallisce per qualche motivo (es. API esterna non disponibile)
                // Potremmo voler restituire un codice di stato più specifico se il ServiceResponse lo indicasse,
                // ma per ora un 500 generico se Success è false e non è stato gestito prima è ok.
                _logger.LogWarning("WeatherService call failed or returned no data. Message: {Message}", response.Message);
                return StatusCode(response.Data == null ? 404 : 500, response); 
            }

            return Ok(response);
        }
    }
}

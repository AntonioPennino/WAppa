// Services/GeocodingService.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; // Richiede il pacchetto System.Net.Http.Json
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherApp.Dtos.Geocoding;

namespace WeatherApp.Services
{
    public class GeocodingService : IGeocodingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GeocodingService> _logger;
        private const string OpenMeteoGeocodingApiUrl = "https://geocoding-api.open-meteo.com/v1/search";

        public GeocodingService(IHttpClientFactory httpClientFactory, ILogger<GeocodingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<GeocodingServiceResultDto>> GetCoordinatesAsync(string query)
        {
            var response = new ServiceResponse<GeocodingServiceResultDto>();
            if (string.IsNullOrWhiteSpace(query))
            {
                response.Success = false;
                response.Message = "Query cannot be empty.";
                return response;
            }

            var httpClient = _httpClientFactory.CreateClient("GeocodingClient"); // Usiamo un client nominato
            // Costruiamo l'URL con i parametri: nome della città, prendi solo il primo risultato (count=1), lingua italiana, formato json
            string requestUrl = $"{OpenMeteoGeocodingApiUrl}?name={Uri.EscapeDataString(query)}&count=1&language=it&format=json";

            try
            {
                var apiResponse = await httpClient.GetFromJsonAsync<OpenMeteoGeocodingResponse>(requestUrl);

                if (apiResponse?.Results != null && apiResponse.Results.Any())
                {
                    var bestResult = apiResponse.Results.First();
                    response.Data = new GeocodingServiceResultDto
                    {
                        Name = bestResult.Name,
                        Latitude = bestResult.Latitude,
                        Longitude = bestResult.Longitude,
                        Country = bestResult.Country,
                        Admin1 = bestResult.Admin1
                    };
                    response.Message = "Coordinates found successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = $"No coordinates found for '{query}'.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching geocoding data for query: {Query}", query);
                response.Success = false;
                response.Message = "An error occurred while fetching geocoding data. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GeocodingService for query: {Query}", query);
                response.Success = false;
                response.Message = "An unexpected error occurred. Please try again later.";
            }

            return response;
        }
    }
}

// Services/WeatherService.cs
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherApp.Dtos.Weather; // Assicurati che questo namespace sia corretto

namespace WeatherApp.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherService> _logger;
        private const string OpenMeteoApiUrl = "https://api.open-meteo.com/v1/forecast";

        public WeatherService(IHttpClientFactory httpClientFactory, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<WeatherForecastDto>> GetWeatherDataAsync(double latitude, double longitude)
        {
            var response = new ServiceResponse<WeatherForecastDto>();
            var httpClient = _httpClientFactory.CreateClient("OpenMeteoClient");

            // Parametri per l'API: includiamo dati correnti e giornalieri
            // Nota: "current" invece di "current_weather" per le nuove API
            // temperature_unit=celsius e wind_speed_unit=kmh sono opzionali, l'API ha dei default
            string currentParams = "temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,snowfall,weather_code,wind_speed_10m,wind_direction_10m";
            string dailyParams = "weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min,sunrise,sunset,precipitation_sum,rain_sum,showers_sum,snowfall_sum,precipitation_probability_max,wind_speed_10m_max,wind_gusts_10m_max,uv_index_max";
            
            // Formattiamo latitudine e longitudine con il punto come separatore decimale, indipendentemente dalla cultura corrente
            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            string requestUrl = $"{OpenMeteoApiUrl}?latitude={latStr}&longitude={lonStr}&current={currentParams}&daily={dailyParams}&timezone=auto&forecast_days=7";
            // Ho usato `current` invece di `current_weather` per allinearsi con le versioni più recenti dell'API.
            // Se `current_weather` funziona meglio per te, puoi cambiarlo e adattare il DTO `OpenMeteoWeatherResponseDto`.

            try
            {
                var apiResponse = await httpClient.GetFromJsonAsync<OpenMeteoWeatherResponseDto>(requestUrl);

                if (apiResponse != null)
                {
                    response.Data = MapToWeatherForecastDto(apiResponse);
                    response.Message = "Weather data retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to retrieve weather data or data was empty.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error fetching weather data for lat: {Latitude}, lon: {Longitude}", latitude, longitude);
                response.Success = false;
                response.Message = "An error occurred while fetching weather data. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in WeatherService for lat: {Latitude}, lon: {Longitude}", latitude, longitude);
                response.Success = false;
                response.Message = "An unexpected error occurred. Please try again later.";
            }

            return response;
        }

        private WeatherForecastDto MapToWeatherForecastDto(OpenMeteoWeatherResponseDto apiData)
        {
            var forecastDto = new WeatherForecastDto
            {
                Latitude = apiData.Latitude,
                Longitude = apiData.Longitude,
                Timezone = apiData.Timezone,
                Current = apiData.CurrentWeather != null ? new CurrentConditionsDto // apiData.Current per nuove API
                {
                    Time = apiData.CurrentWeather.Time,
                    Temperature = apiData.CurrentWeather.Temperature,
                    ApparentTemperature = apiData.CurrentWeather.ApparentTemperature,
                    RelativeHumidity = apiData.CurrentWeather.RelativeHumidity,
                    Precipitation = apiData.CurrentWeather.Precipitation,
                    WeatherCode = apiData.CurrentWeather.WeatherCode,
                    WindSpeed = apiData.CurrentWeather.WindSpeed,
                    WeatherDescription = GetWeatherDescription(apiData.CurrentWeather.WeatherCode)
                } : null,
                Daily = new List<DailyForecastDto>()
            };

            if (apiData.Daily?.Time != null && apiData.Daily.Time.Any())
            {
                for (int i = 0; i < apiData.Daily.Time.Count; i++)
                {
                    forecastDto.Daily.Add(new DailyForecastDto
                    {
                        Date = apiData.Daily.Time[i],
                        TemperatureMax = apiData.Daily.TemperatureMax[i],
                        TemperatureMin = apiData.Daily.TemperatureMin[i],
                        ApparentTemperatureMax = apiData.Daily.ApparentTemperatureMax[i],
                        ApparentTemperatureMin = apiData.Daily.ApparentTemperatureMin[i],
                        WeatherCode = apiData.Daily.WeatherCode[i],
                        Sunrise = apiData.Daily.Sunrise[i],
                        Sunset = apiData.Daily.Sunset[i],
                        PrecipitationSum = apiData.Daily.PrecipitationSum[i],
                        PrecipitationProbabilityMax = apiData.Daily.PrecipitationProbabilityMax.Count > i ? apiData.Daily.PrecipitationProbabilityMax[i] : null,
                        WindSpeedMax = apiData.Daily.WindSpeedMax[i],
                        WeatherDescription = GetWeatherDescription(apiData.Daily.WeatherCode[i])
                    });
                }
            }
            return forecastDto;
        }

        // Semplice mappatura del WMO Weather code alla descrizione (puoi espanderla)
        // Fonte: https://open-meteo.com/en/docs WMO Weather interpretation codes (WW)
        private string GetWeatherDescription(int weatherCode)
        {
            return weatherCode switch
            {
                0 => "Sereno",
                1 => "Prevalentemente sereno",
                2 => "Parzialmente nuvoloso",
                3 => "Nuvoloso",
                45 => "Nebbia",
                48 => "Nebbia con brina",
                51 => "Pioggerella leggera",
                53 => "Pioggerella moderata",
                55 => "Pioggerella densa",
                56 => "Pioggerella gelata leggera",
                57 => "Pioggerella gelata densa",
                61 => "Pioggia leggera",
                63 => "Pioggia moderata",
                65 => "Pioggia forte",
                66 => "Pioggia gelata leggera",
                67 => "Pioggia gelata forte",
                71 => "Nevicata leggera",
                73 => "Nevicata moderata",
                75 => "Nevicata forte",
                77 => "Gragnola",
                80 => "Rovescio leggero",
                81 => "Rovescio moderato",
                82 => "Rovescio violento",
                85 => "Rovescio di neve leggero",
                86 => "Rovescio di neve forte",
                95 => "Temporale leggero o moderato",
                96 => "Temporale con grandine leggera",
                99 => "Temporale con grandine forte",
                _ => "Non disponibile"
            };
        }
    }
}

// Dtos/Weather/OpenMeteoDailyDataDto.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    public class OpenMeteoDailyDataDto
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new List<string>();

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; } = new List<int>();

        [JsonPropertyName("temperature_2m_max")]
        public List<double> TemperatureMax { get; set; } = new List<double>();

        [JsonPropertyName("temperature_2m_min")]
        public List<double> TemperatureMin { get; set; } = new List<double>();

        [JsonPropertyName("apparent_temperature_max")]
        public List<double> ApparentTemperatureMax { get; set; } = new List<double>();

        [JsonPropertyName("apparent_temperature_min")]
        public List<double> ApparentTemperatureMin { get; set; } = new List<double>();

        [JsonPropertyName("sunrise")]
        public List<string> Sunrise { get; set; } = new List<string>();

        [JsonPropertyName("sunset")]
        public List<string> Sunset { get; set; } = new List<string>();

        [JsonPropertyName("precipitation_sum")]
        public List<double> PrecipitationSum { get; set; } = new List<double>();

        [JsonPropertyName("rain_sum")]
        public List<double> RainSum { get; set; } = new List<double>();

        [JsonPropertyName("showers_sum")]
        public List<double> ShowersSum { get; set; } = new List<double>();

        [JsonPropertyName("snowfall_sum")]
        public List<double> SnowfallSum { get; set; } = new List<double>();

        [JsonPropertyName("precipitation_probability_max")]
        public List<int?> PrecipitationProbabilityMax { get; set; } = new List<int?>(); // Può essere null

        [JsonPropertyName("wind_speed_10m_max")]
        public List<double> WindSpeedMax { get; set; } = new List<double>();

        [JsonPropertyName("wind_gusts_10m_max")]
        public List<double> WindGustsMax { get; set; } = new List<double>();

        [JsonPropertyName("uv_index_max")]
        public List<double?> UvIndexMax { get; set; } = new List<double?>(); // Può essere null

        // Aggiungi qui altre liste di dati se richiedi più dati giornalieri
    }
}

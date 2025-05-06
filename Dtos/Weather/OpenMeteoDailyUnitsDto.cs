// Dtos/Weather/OpenMeteoDailyUnitsDto.cs
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    public class OpenMeteoDailyUnitsDto
    {
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("weather_code")]
        public string WeatherCode { get; set; } = string.Empty;

        [JsonPropertyName("temperature_2m_max")]
        public string TemperatureMax { get; set; } = string.Empty;

        [JsonPropertyName("temperature_2m_min")]
        public string TemperatureMin { get; set; } = string.Empty;

        [JsonPropertyName("apparent_temperature_max")]
        public string ApparentTemperatureMax { get; set; } = string.Empty;

        [JsonPropertyName("apparent_temperature_min")]
        public string ApparentTemperatureMin { get; set; } = string.Empty;

        [JsonPropertyName("sunrise")]
        public string Sunrise { get; set; } = string.Empty;

        [JsonPropertyName("sunset")]
        public string Sunset { get; set; } = string.Empty;

        [JsonPropertyName("precipitation_sum")]
        public string PrecipitationSum { get; set; } = string.Empty;

        [JsonPropertyName("rain_sum")]
        public string RainSum { get; set; } = string.Empty;

        [JsonPropertyName("showers_sum")]
        public string ShowersSum { get; set; } = string.Empty;

        [JsonPropertyName("snowfall_sum")]
        public string SnowfallSum { get; set; } = string.Empty;

        [JsonPropertyName("precipitation_probability_max")]
        public string PrecipitationProbabilityMax { get; set; } = string.Empty;

        [JsonPropertyName("wind_speed_10m_max")]
        public string WindSpeedMax { get; set; } = string.Empty;

        [JsonPropertyName("wind_gusts_10m_max")]
        public string WindGustsMax { get; set; } = string.Empty;

        [JsonPropertyName("uv_index_max")]
        public string UvIndexMax { get; set; } = string.Empty;

        // Aggiungi qui altre unità se richiedi più dati giornalieri
    }
}

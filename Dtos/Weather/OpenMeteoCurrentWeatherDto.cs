// Dtos/Weather/OpenMeteoCurrentWeatherDto.cs
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    public class OpenMeteoCurrentWeatherDto
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public double ApparentTemperature { get; set; }

        [JsonPropertyName("is_day")]
        public int IsDay { get; set; } // 1 for day, 0 for night

        [JsonPropertyName("precipitation")]
        public double Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public double Rain { get; set; }

        [JsonPropertyName("showers")]
        public double Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public double Snowfall { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public int WindDirection { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;
    }
}

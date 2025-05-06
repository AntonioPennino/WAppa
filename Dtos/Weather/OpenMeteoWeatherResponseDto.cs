// Dtos/Weather/OpenMeteoWeatherResponseDto.cs
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    public class OpenMeteoWeatherResponseDto
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; } = string.Empty;

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("current_weather")] // "current" in nuove versioni API, ma manteniamo "current_weather" per compatibilità o verifichiamo l'ultima API
        public OpenMeteoCurrentWeatherDto? CurrentWeather { get; set; } // Nota: potrebbe essere "current" invece di "current_weather"

        [JsonPropertyName("daily_units")]
        public OpenMeteoDailyUnitsDto? DailyUnits { get; set; }

        [JsonPropertyName("daily")]
        public OpenMeteoDailyDataDto? Daily { get; set; }
    }
}

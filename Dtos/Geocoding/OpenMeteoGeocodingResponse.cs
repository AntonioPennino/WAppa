// Dtos/Geocoding/OpenMeteoGeocodingResponse.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Geocoding
{
    public class OpenMeteoGeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<OpenMeteoGeocodingResult>? Results { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }
    }
}

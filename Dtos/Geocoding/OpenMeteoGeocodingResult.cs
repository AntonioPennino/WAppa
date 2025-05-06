// Dtos/Geocoding/OpenMeteoGeocodingResult.cs
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Geocoding
{
    public class OpenMeteoGeocodingResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("admin1")]
        public string? Admin1 { get; set; } // Regione o stato, può essere nullo
    }
}

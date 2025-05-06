// Dtos/Geocoding/GeocodingServiceResultDto.cs
namespace WeatherApp.Dtos.Geocoding
{
    public class GeocodingServiceResultDto
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? Admin1 { get; set; } // Regione o stato
    }
}

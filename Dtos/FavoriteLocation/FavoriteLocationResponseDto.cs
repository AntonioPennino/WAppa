// Dtos/FavoriteLocation/FavoriteLocationResponseDto.cs
using WeatherApp.Dtos.Weather; // Per includere WeatherForecastDto o una sua versione semplificata

namespace WeatherApp.Dtos.FavoriteLocation
{
    public class FavoriteLocationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public WeatherForecastDto? WeatherData { get; set; } // Dati meteo per questa località
    }
}

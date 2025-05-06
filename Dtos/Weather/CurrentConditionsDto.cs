// Dtos/Weather/CurrentConditionsDto.cs
namespace WeatherApp.Dtos.Weather
{
    public class CurrentConditionsDto
    {
        public string Time { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double ApparentTemperature { get; set; }
        public int RelativeHumidity { get; set; }
        public double Precipitation { get; set; }
        public int WeatherCode { get; set; } // Per le icone
        public double WindSpeed { get; set; }
        public string? WeatherDescription { get; set; } // Aggiungeremo una mappatura per questo
    }
}

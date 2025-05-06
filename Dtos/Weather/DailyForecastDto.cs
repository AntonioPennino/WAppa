// Dtos/Weather/DailyForecastDto.cs
namespace WeatherApp.Dtos.Weather
{
    public class DailyForecastDto
    {
        public string Date { get; set; } = string.Empty;
        public double TemperatureMax { get; set; }
        public double TemperatureMin { get; set; }
        public double ApparentTemperatureMax { get; set; }
        public double ApparentTemperatureMin { get; set; }
        public int WeatherCode { get; set; } // Per le icone
        public string Sunrise { get; set; } = string.Empty;
        public string Sunset { get; set; } = string.Empty;
        public double PrecipitationSum { get; set; }
        public int? PrecipitationProbabilityMax { get; set; }
        public double WindSpeedMax { get; set; }
        public string? WeatherDescription { get; set; } // Aggiungeremo una mappatura per questo
    }
}

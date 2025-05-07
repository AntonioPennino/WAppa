// Dtos/Weather/WeatherForecastDto.cs
using System.Collections.Generic;

namespace WeatherApp.Dtos.Weather
{
    public class WeatherForecastDto
    {
        public string LocationName { get; set; } = string.Empty; // NUOVO CAMPO AGGIUNTO
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public CurrentConditionsDto? Current { get; set; }
        public List<DailyForecastDto> Daily { get; set; } = new List<DailyForecastDto>();
    }
}

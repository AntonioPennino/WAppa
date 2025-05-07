// Services/IWeatherService.cs
using System.Threading.Tasks;
using WeatherApp.Dtos.Weather;

namespace WeatherApp.Services
{
    public interface IWeatherService
    {
        // AGGIUNTO il parametro opzionale locationNameFromGeocoding
        Task<ServiceResponse<WeatherForecastDto>> GetWeatherDataAsync(double latitude, double longitude, string? locationNameFromGeocoding = null);
    }
}

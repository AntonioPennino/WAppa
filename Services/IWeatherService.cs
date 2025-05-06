// Services/IWeatherService.cs
using System.Threading.Tasks;
using WeatherApp.Dtos.Weather;

namespace WeatherApp.Services
{
    public interface IWeatherService
    {
        Task<ServiceResponse<WeatherForecastDto>> GetWeatherDataAsync(double latitude, double longitude);
    }
}

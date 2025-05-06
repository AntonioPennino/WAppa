// Services/IGeocodingService.cs
using System.Threading.Tasks;
using WeatherApp.Dtos.Geocoding;

namespace WeatherApp.Services
{
    public interface IGeocodingService
    {
        Task<ServiceResponse<GeocodingServiceResultDto>> GetCoordinatesAsync(string query);
    }
}

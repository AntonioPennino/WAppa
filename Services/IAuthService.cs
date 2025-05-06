// Services/IAuthService.cs
using System.Threading.Tasks;
using WeatherApp.Dtos.Auth;
using WeatherApp.Models; // Per accedere al modello User

namespace WeatherApp.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<ServiceResponse<AuthResponseDto>> Login(string username, string password);
        // Task<bool> UserExists(string username); // Potrebbe essere utile, ma per ora lo mettiamo nel register
    }
}

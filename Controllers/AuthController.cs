// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeatherApp.Dtos.Auth;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Costruttore riportato alla sua forma originale, solo con IAuthService
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var user = new User { Username = request.Username };
            var response = await _authService.Register(user, request.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var response = await _authService.Login(request.Username, request.Password);

            if (!response.Success)
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }

        // Il metodo TestWeather e la dipendenza IWeatherService sono stati rimossi
    }
}

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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var user = new User { Username = request.Username }; // Mappiamo dal DTO al modello User
            var response = await _authService.Register(user, request.Password);

            if (!response.Success)
            {
                return BadRequest(response); // Restituisce un 400 con il messaggio di errore
            }
            return Ok(response); // Restituisce un 200 con i dati (ID utente)
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var response = await _authService.Login(request.Username, request.Password);

            if (!response.Success)
            {
                return Unauthorized(response); // Restituisce un 401 con il messaggio di errore
            }
            return Ok(response); // Restituisce un 200 con il token e username
        }
    }
}

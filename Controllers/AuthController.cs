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
        private readonly IGeocodingService _geocodingService;

        // Modifica il costruttore per iniettare anche IGeocodingService
        public AuthController(IAuthService authService, IGeocodingService geocodingService)
        {
            _authService = authService;
            _geocodingService = geocodingService;
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

        // Nuovo endpoint di test per il servizio di geocodifica
        // Esempio: GET /api/auth/geocode?query=Roma
        [HttpGet("geocode")]
        public async Task<IActionResult> TestGeocode([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { success = false, message = "Query parameter is required." });
            }

            var response = await _geocodingService.GetCoordinatesAsync(query);

            if (!response.Success || response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}

// Dtos/Auth/UserLoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Dtos.Auth
{
    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

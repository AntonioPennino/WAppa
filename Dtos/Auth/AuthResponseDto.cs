// Dtos/Auth/AuthResponseDto.cs
namespace WeatherApp.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        // Puoi aggiungere altre informazioni sull'utente se necessario
    }
}

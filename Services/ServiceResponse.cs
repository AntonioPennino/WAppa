// Services/ServiceResponse.cs (o Models/ServiceResponse.cs)
namespace WeatherApp.Services // o WeatherApp.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}

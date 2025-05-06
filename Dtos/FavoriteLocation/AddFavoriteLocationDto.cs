// Dtos/FavoriteLocation/AddFavoriteLocationDto.cs
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Dtos.FavoriteLocation
{
    public class AddFavoriteLocationDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Location name must be between 2 and 100 characters.")]
        public string LocationName { get; set; } = string.Empty; // Es. "Roma", "New York, NY", "90210"
    }
}

// Models/User.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Proprietà di navigazione per le località preferite
        public virtual ICollection<FavoriteLocation> FavoriteLocations { get; set; } = new List<FavoriteLocation>();
    }
}
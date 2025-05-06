// Models/FavoriteLocation.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherApp.Models
{
    public class FavoriteLocation
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // Es. "Roma, Italia"

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        // Chiave esterna per l'utente
        public int UserId { get; set; }

        // Proprietà di navigazione verso l'utente
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
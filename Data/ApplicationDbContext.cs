// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models; // Assicurati che il namespace dei tuoi modelli sia corretto

namespace WeatherApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FavoriteLocation> FavoriteLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazione per rendere lo Username univoco
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Configurazione della relazione uno-a-molti tra User e FavoriteLocation
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteLocations)
                .WithOne(fl => fl.User) // L'utente a cui appartiene la località
                .HasForeignKey(fl => fl.UserId) // La chiave esterna in FavoriteLocation
                .OnDelete(DeleteBehavior.Cascade); // Se un utente viene cancellato, cancella anche le sue località preferite
        }
    }
}
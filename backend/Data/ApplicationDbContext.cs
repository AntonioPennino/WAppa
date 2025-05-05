using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurazioni aggiuntive del modello possono essere aggiunte qui
            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 1, Name = "Elemento di esempio", Description = "Questo è un elemento di esempio", IsComplete = false }
            );
        }
    }
}
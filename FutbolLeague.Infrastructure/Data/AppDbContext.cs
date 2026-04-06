using Microsoft.EntityFrameworkCore;
using FutbolLeague.Domain;

namespace FutbolLeague.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } // Assuming Category is defined in the domain layer
        public DbSet<Team> Teams { get; set; } // Assuming Team is defined in the domain layer
        public DbSet<Match> Matches { get; set; } // Assuming Match is defined in the domain layer
        public DbSet<Tournament> Tournaments { get; set; } // Assuming Tournament is defined in the domain layer

        
        //
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Esto va arriba por proligidad
            base.OnModelCreating(modelBuilder);
            // Configure entity relationships and constraints here if needed
            // For example:
            // modelBuilder.Entity<Team>()
            //     .HasOne(t => t.Category)
            //     .WithMany(c => c.Teams)
            //     .HasForeignKey(t => t.CategoryId);
           
            //Validacion 
            modelBuilder.Entity<Tournament>()
                .HasIndex(t => t.Name)
                .IsUnique(); // Ensure tournament names are unique
        }
    }
}
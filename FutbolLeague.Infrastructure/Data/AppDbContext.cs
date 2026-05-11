using Microsoft.EntityFrameworkCore;
using FutbolLeague.Domain;
using System.Security.Cryptography;

namespace FutbolLeague.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        //Que es un DbContext?
        //Es la clase principal que se utiliza para interactuar con la base de datos en Entity Framework Core.
        //Proporciona una forma de consultar y guardar datos en la base de datos, y también se encarga de administrar las conexiones a la base de datos y el seguimiento de los cambios en los objetos.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Que hace un DbSet? 
        // Un DbSet representa una colección de entidades de un tipo específico que se pueden consultar y guardar en la base de datos.
        // Cada DbSet corresponde a una tabla en la base de datos, y cada entidad dentro del DbSet corresponde a una fila en esa tabla.
        // En este caso, estamos definiendo DbSet para cada una de las entidades principales de nuestro dominio, lo que nos permitirá realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en esas entidades a través del contexto de la base de datos.
        public DbSet<Category> Categories { get; set; } // Assuming Category is defined in the domain layer
        public DbSet<Team> Teams { get; set; } // Assuming Team is defined in the domain layer
        public DbSet<Match> Matches { get; set; } // Assuming Match is defined in the domain layer
        public DbSet<Tournament> Tournaments { get; set; } // Assuming Tournament is defined in the domain layer
        public DbSet<Field> Fields { get; set; } // Assuming Field is defined in the domain layer
        public DbSet<User> Users { get; set; } // Assuming User is defined in the domain layer

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

            // Agregar un unique index en el UserName para asegurar que no haya usuarios con el mismo nombre de usuario
            // evita duplicado admin admin, admin1, admin2, etc.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique(); // Ensure usernames are unique
        }
    }
}
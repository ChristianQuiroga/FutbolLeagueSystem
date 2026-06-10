
namespace FutbolLeague.Application.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } // Agrega esta propiedad para indicar si la categoría tiene equipos activos
    }
}   
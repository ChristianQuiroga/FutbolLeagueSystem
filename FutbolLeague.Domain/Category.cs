namespace FutbolLeague.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true; // Default to active when created
    }
}
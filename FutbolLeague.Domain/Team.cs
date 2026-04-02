namespace FutbolLeague.Domain
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }  // ? hace que no sea obligatorio, es decir, puede ser null
    }
}
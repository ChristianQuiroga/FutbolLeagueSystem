namespace FutbolLeague.Domain
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }


        // Relación con la categoría a la que pertenece el equipo
        public int CategoryId { get; set; }
        public Category? Category { get; set; }  // ? hace que no sea obligatorio, es decir, puede ser null


        // Relación con el torneo al que pertenece el equipo
        public int TournamentId { get; set; }
        public Tournament? Tournament { get; set; } // Tournament es la clase que representa el torneo al que pertenece el equipo

    }
}
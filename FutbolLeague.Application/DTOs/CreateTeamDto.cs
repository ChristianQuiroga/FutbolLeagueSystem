namespace FutbolLeague.Application.DTOs
{
    public class CreateTeamDto
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int TournamentId { get; set; } // TournamentId es el ID del torneo al que pertenece el equipo

    }
}
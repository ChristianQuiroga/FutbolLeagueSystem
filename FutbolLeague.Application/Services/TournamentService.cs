using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Exceptions;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.Application.Services
{
    //Creamos la implementación de la interfaz ITournamentService, que se encargará de manejar la lógica de negocio relacionada con los torneos. Esta clase se encargará de interactuar con el repositorio o la base de datos para realizar las operaciones necesarias, como crear un torneo, listar los torneos disponibles y obtener un resumen del torneo por categoría.

    public class TournamentService: ITournamentService
    {
        private readonly AppDbContext _context;
        private readonly IStandingService _standingService;

        // Inyectamos el contexto de la base de datos y el servicio de clasificación (standingService) a través del constructor para poder utilizarlos en los métodos de la clase.
        public TournamentService(AppDbContext context, IStandingService standingService)
        {
            _context = context;
            _standingService = standingService;
        }

        // Aquí irían las implementaciones de los métodos definidos en la interfaz ITournamentService, como GetAllAsync, CreateAsync y GetSummaryAsync. Estos métodos se encargarían de realizar las operaciones necesarias utilizando el contexto de la base de datos y el servicio de clasificación (standingService) para obtener la información requerida.
        public async Task<List<TournamentDto>> GetAllAsync()
        {
            // Implementación para listar torneos
            return await _context.Tournaments
                .Select(t => new TournamentDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    FixtureFormat = (int)t.FixtureFormat,
                    FixtureFormatName = t.FixtureFormat.ToString() // Asumiendo que FixtureFormat es un enum
                })
                .ToListAsync();
        }

        // Implementación para crear un nuevo torneo
        public async Task<TournamentDto> CreateAsync(CreateTournamentDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                throw new BusinessException("El nombre es obligatorio");

            if (!Enum.IsDefined(typeof(FixtureFormat), dto.FixtureFormat))
                throw new BusinessException("Formato de fixture inválido");

            var exists = await _context.Tournaments
                .AnyAsync(t => t.Name.ToLower() == dto.Name.ToLower());
            if (exists)
                throw new ConflictException("Ya existe un torneo con ese nombre");

            var tournament = new Tournament
            {
                Name = dto.Name.Trim(),
                FixtureFormat = (FixtureFormat)dto.FixtureFormat // Asumiendo que el dto.FixtureFormat es un entero que representa el valor del enum FixtureFormat
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync(); // Guardamos los cambios en la base de datos para que el nuevo torneo se persista.

            return new TournamentDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                FixtureFormat = (int)tournament.FixtureFormat,
                FixtureFormatName = tournament.FixtureFormat.ToString()
            }; // Devolvemos un objeto TournamentDto con la información del torneo recién creado, incluyendo su ID generado por la base de datos, el nombre, el formato de fixture y el nombre del formato de fixture.
        }

        // Implementación para obtener el resumen de un torneo por categoría
        public async Task<object> GetSummaryAsync(int tournamentId, int categoryId)
        {
            // Aquí iría la lógica para obtener el resumen del torneo por categoría, incluyendo próximos partidos, últimos resultados, standings y estadísticas.
            // Esto podría implicar consultas a la base de datos para obtener la información relevante y luego formatearla en un objeto que se devuelva al controlador.
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId && t.CategoryId == categoryId)
                .ToListAsync();

            if (!teams.Any())
               throw new NotFoundException("No hay equipos registrados para este torneo y categoría");

            var teamIds = teams.Select(t => t.Id).ToList(); // Obtenemos los IDs de los equipos para luego consultar los partidos relacionados con esos equipos.

            var matches = await _context.Matches
                .Include(m => m.HomeTeam) // Incluimos la información del equipo local para poder mostrarla en el resumen.
                .Include(m => m.AwayTeam) // Incluimos la información del equipo visitante para poder mostrarla en el resumen.
                .Include(m => m.Field) // Incluimos la información del campo para poder mostrarla en el resumen.
                .Where(m => m.TournamentId == tournamentId &&
                            teamIds.Contains(m.HomeTeamId) &&
                            teamIds.Contains(m.AwayTeamId)) // Filtramos los partidos que pertenecen al torneo especificado y que involucran a los equipos de la categoría especificada, asegurándonos de incluir tanto al equipo local como al visitante en la consulta para obtener toda la información necesaria para el resumen.
                .ToListAsync(); // Obtenemos los partidos relacionados con los equipos del torneo y categoría especificados.
            
            if (!matches.Any() && !teams.Any())
                throw new NotFoundException("No hay partidos registrados para este torneo y categoría");

            var nextMatches = matches
               .Where(m => m.Status == MatchStatus.Pending)
               .OrderBy(m => m.MatchDate)
               .ThenBy(m => m.Round)
               .Take(5)
               .Select(m => new MatchDto
               {
                   Id = m.Id,
                   Round = m.Round,
                   HomeTeam = m.HomeTeam!.Name,
                   AwayTeam = m.AwayTeam!.Name,
                   HomeScore = m.HomeScore,
                   AwayScore = m.AwayScore,
                   Status = m.Status.ToString(),
                   MatchDate = m.MatchDate,
                   Field = m.Field != null ? m.Field.Name : null
               })
               .ToList();

            var lastResults = matches
                .Where(m => m.Status == MatchStatus.Played)
                .OrderByDescending(m => m.MatchDate)
                .ThenByDescending(m => m.Id)
                .Take(5)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam!.Name,
                    AwayTeam = m.AwayTeam!.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(),
                    MatchDate = m.MatchDate,
                    Field = m.Field != null ? m.Field.Name : null
                })
                .ToList();

            // Obtenemos la tabla de posiciones (standings) para el torneo y categoría especificados utilizando el servicio de clasificación (standingService) que se inyectó en el constructor de la clase. Este servicio se encargará de calcular la tabla de posiciones basada en los resultados de los partidos y la información de los equipos.
            var standings = await _standingService
                .GetStandingsByTournamentAndCategoryAsync(tournamentId, categoryId);

            var totalMatches = matches.Count;
            var playedMatches = matches.Count(m => m.Status == MatchStatus.Played);
            var pendingMatches = matches.Count(m => m.Status == MatchStatus.Pending);

            return new
            {
                TournamentId = tournamentId,
                CategoryId = categoryId,
                NextMatches = nextMatches,
                LastResults = lastResults,
                Standings = standings,
                Stats = new
                {
                    TotalMatches = totalMatches,
                    PlayedMatches = playedMatches,
                    PendingMatches = pendingMatches,
                    TeamsCount = teams.Count
                }
            };            
        }

    }
}

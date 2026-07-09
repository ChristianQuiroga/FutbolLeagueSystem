using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Exceptions;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly AppDbContext _context;

        public TeamService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeamDto>> GetAllAsync()
        {
            return await _context.Teams
                .Include(t => t.Category)
                .Include(t => t.Tournament)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category!.Name,
                    TournamentId = t.TournamentId,
                    TournamentName = t.Tournament!.Name
                })
                .ToListAsync();
        }

        public async Task<TeamDto> CreateAsync(CreateTeamDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BusinessException("El nombre del equipo es obligatorio");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.IsActive);

            if (category == null)
                throw new NotFoundException("La categoría no existe o está desactivada");

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == dto.TournamentId);

            if (tournament == null)
                throw new NotFoundException("El torneo no existe");

            if (tournament.Status == TournamentStatus.Finished)
                throw new ConflictException("No se pueden agregar equipos a un torneo finalizado");

            var teamExists = await _context.Teams
                .AnyAsync(t =>
                    t.Name.ToLower() == dto.Name.ToLower()
                    && t.CategoryId == dto.CategoryId
                    && t.TournamentId == dto.TournamentId);

            if (teamExists)
                throw new ConflictException("Ya existe un equipo con ese nombre en la misma categoría y torneo");

            var team = new Team
            {
                Name = dto.Name.Trim(),
                CategoryId = dto.CategoryId,
                TournamentId = dto.TournamentId
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                CategoryId = team.CategoryId,
                CategoryName = category.Name,
                TournamentId = team.TournamentId,
                TournamentName = tournament.Name
            };
        }
    }
}
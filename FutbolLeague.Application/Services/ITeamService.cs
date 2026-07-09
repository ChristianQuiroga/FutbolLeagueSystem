using FutbolLeague.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.Services
{
    public interface ITeamService
    {
        Task<List<TeamDto>> GetAllAsync();
        Task<TeamDto> CreateAsync(CreateTeamDto dto);
    }
}

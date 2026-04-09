using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class UpdateMatchResultDto //Validar los resultados
    {
        public int HomeScore { get; set; } //Goles loca
        public int AwayScore { get; set; } //Goles Visitante
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    // DTO para generar el fixture de un torneo por categoría
    public class GenerateFixtureByCategoryDto
    {
        public int TournamentId { get; set; } // Id del torneo para el cual se generará el fixture
        public int CategoryId { get; set; } // Id de la categoría para la cual se generará el fixture

    }
}

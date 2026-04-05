using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class CreateMatchDto
    {
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
    }
}

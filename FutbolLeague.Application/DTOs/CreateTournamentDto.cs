using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class CreateTournamentDto
    {
        public string Name { get; set; }
        public int FixtureFormat { get; set; } // Store the integer value of the FixtureFormat enum

    }
}

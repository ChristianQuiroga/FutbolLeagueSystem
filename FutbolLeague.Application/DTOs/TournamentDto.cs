using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class TournamentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FixtureFormat { get; set; } // Store the integer value of the FixtureFormat enum
        public string FixtureFormatName { get; set; } // Optional: Include the name of the fixture format for easier display
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}

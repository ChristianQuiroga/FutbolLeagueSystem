using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Domain
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FixtureFormat FixtureFormat { get; set; } // Assuming FixtureFormat is an enum defined elsewhere in the domain

    }
}

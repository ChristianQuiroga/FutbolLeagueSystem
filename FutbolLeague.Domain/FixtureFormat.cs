using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Domain
{
    // Enum representing different fixture formats for a football league
    public enum FixtureFormat
    {
        //None = 0 //Valor por defecto, no se ha establecido el formato del fixture
        SingleRoundRobin = 1,
        DoubleRoundRobin = 2,
        //Knockout = 3,
    }
}

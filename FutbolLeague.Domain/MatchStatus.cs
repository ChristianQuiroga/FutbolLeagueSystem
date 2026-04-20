using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Domain
{
    public enum MatchStatus
    {
        //None = 0, //Valor por defecto, no se ha establecido el estado del partido
        Pending = 1,
        Played = 2,
    }
}

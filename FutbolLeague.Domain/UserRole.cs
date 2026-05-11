using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Domain
{
    //Porque Enum porque evita usar strings sueltos y es mas facil de manejar, ademas de que es mas eficiente en
    //terminos de memoria y rendimiento. 
    public enum UserRole
    {
        User = 1, Admin = 2
    }
}

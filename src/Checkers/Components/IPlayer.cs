using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public interface IPlayer
    {
        Player.EnumColor Color { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Players<T1, T2> where T1 : Player, new()
                                where T2 : Player, new()
    {
        #region Public Properties

        internal T1 One { get; set; }
        internal T2 Two { get; set; }

        #endregion Public Properties

        #region Public Methods

        internal static Players<T1, T2> createPlayers() => new Players<T1, T2>
        {
            One = new T1(),
            Two = new T2()
        };

        internal Player getPlayerFromColor(Player.EnumColor color)
        {
            if (One.Color == color) return One;
            else return Two;
        }

        #endregion Public Methods
    }
}
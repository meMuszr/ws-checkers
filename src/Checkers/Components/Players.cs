using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Players<T1, T2> where T1 : Player, new()
                                where T2 : Player, new()
    {
        public T1 One { get; set; }
        public T2 Two { get; set; }

        public static Players<T1, T2> createPlayers()
        {
            return new Players<T1, T2>
            {
                One = new T1(),
                Two = new T2()
            };
        }
        public Player getPlayerFromColor(Player.EnumColor color)
        {
            if (One.Color == color) return One;
            else return Two;
        }
    }
}

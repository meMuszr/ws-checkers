using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Players<T1,T2> where T1: IPlayer, new()
                                where T2: IPlayer, new()
    {
        public T1 One { get; set; }
        public T2 Two { get; set; }

        public static Players<T1,T2> createPlayers()
        {
            return new Players<T1, T2>
            {
                One = new T1(),
                Two = new T2()
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Point
    {
        public byte X { get; private set; }
        public byte Y { get; private set; }
        public Point setPoint(byte x, byte y)
        {
            X = x;
            Y = y;
            return this;
        }
        public Point(byte x, byte y)
        {
            X = x;
            Y = y;
        }
    }
}

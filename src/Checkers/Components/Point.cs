using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Point
    {
        private int x;
        private int y;

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

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }
        public override bool Equals(object obj)
        {
            var point = (Point)obj;
            return X == point.X && Y == point.Y;
        }
    }
}

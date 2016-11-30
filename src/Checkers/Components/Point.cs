using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Point
    {
        #region Private Constructors

        public Point(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public Point(int x, int y)
        {
            X = (byte)x;
            Y = (byte)y;
        }

        #endregion Private Constructors

        #region Internal Properties

        internal byte X { get; private set; }
        internal byte Y { get; private set; }

        #endregion Internal Properties

        #region Public Methods

        public override bool Equals(object obj)
        {
            var point = (Point)obj;
            return X == point.X && Y == point.Y;
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

        #endregion Public Methods

        #region Internal Methods

        internal Point setPoint(byte x, byte y)
        {
            X = x;
            Y = y;
            return this;
        }

        #endregion Internal Methods
    }
}
using Checkers.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Board
    {
        #region Public Fields

        internal static readonly byte GRID_SIZE = 8;

        #endregion Public Fields

        #region Private Fields

        private static Dictionary<Direction, Func<bool, bool>> directionFunc = new Dictionary<Direction, Func<bool, bool>>();

        #endregion Private Fields

        #region Public Enums

        internal enum Direction
        {
            NE = 1, NW = 2, SW = 3, SE = 4
        }

        #endregion Public Enums

        #region Public Properties

        internal byte?[,] Grid { get; } = new byte?[8, 8];

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    if (!Grid[y, x].HasValue)
                    {
                        sb.Append("_");
                    }
                    else
                    {
                        var stringValue = Grid[y, x].ToString();
                        switch ((Player.EnumColor)(int.Parse(stringValue[0].ToString())))
                        {
                            case Player.EnumColor.White:
                                sb.Append("X");
                                break;

                            case Player.EnumColor.Black:
                                sb.Append("O");
                                break;
                        }
                    }
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }

        internal void addToGrid(int x, int y, Player.EnumColor color, Piece.PieceType pieceType)
        {
            if (Grid[y, x] == null) Grid[y, x] = PieceInfoToByte(color, pieceType);
        }

        internal Direction getMoveDirection(int dY, int dX)
        {
            var location = Math.Atan2(dY, dX);
            if (location > Math.PI / 2) return Direction.NW;
            else if (location < -Math.PI / 2) return Direction.SW;
            else if (location < 0) return Direction.SE;
            else return Direction.NE;
        }

        internal void relocatePoint(Point originalPoint, Point newPoint)
        {
            Grid[newPoint.Y, newPoint.X] = Grid[originalPoint.Y, originalPoint.X].Value;
            Grid[originalPoint.Y, originalPoint.X] = null;
        }

        internal void removePoint(Point point)
        {
            Grid[point.Y, point.X] = null;
        }

        #endregion Public Methods

        #region Private Methods

        //TODO -
        //Add conditional for if location is used / empty --done
        //Add check for jumps
        private bool canMove(Piece piece, Point newCoordinates)
        {
            bool rtnBool = false;
            switch (piece.TypeOfPiece)
            {
                case Piece.PieceType.Man:
                    rtnBool |= (piece.Location.X + 1 == newCoordinates.X || piece.Location.X - 1 == newCoordinates.X) &&
                               (piece.Location.Y + 1 == newCoordinates.Y);
                    break;

                case Piece.PieceType.King:
                    rtnBool |= (piece.Location.X + 1 == newCoordinates.X || piece.Location.X - 1 == newCoordinates.X) &&
                               (piece.Location.Y + 1 == newCoordinates.Y || piece.Location.Y - 1 == newCoordinates.Y);
                    break;

                default:
                    rtnBool |= false;
                    break;
            }
            return rtnBool;
        }

        private Point isNewCoordinateValid(Point point)
        {
            if (point == null) throw new ArgumentNullException();
            if (point.X >= Grid.Length || point.Y >= Grid.Length) throw new ArgumentException();
            else return point;
        }

        private byte PieceInfoToByte(Player.EnumColor color, Piece.PieceType pieceType) => byte.Parse($"{(int)color}{(int)pieceType}");

        #endregion Private Methods
    }
}
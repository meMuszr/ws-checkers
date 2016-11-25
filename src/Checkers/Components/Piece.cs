using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Piece
    {
        public PieceType TypeOfPiece { get; private set; }
        public Point Location { get; private set; }
        public Piece movePiece(byte x, byte y)
        {
            Location.setPoint(x, y);
            return this;
        }
        public Piece(Point point, PieceType type)
        {
            Location = point;
            TypeOfPiece = type;
        }
        public enum PieceType
        {
            Man = 1,
            King = 2
        }
    }
}

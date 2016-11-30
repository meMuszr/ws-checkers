using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Checkers.Components.Player;

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
        public static IEnumerable<Piece> createPieces(EnumColor color)
        {
            switch (color)
            {
                case EnumColor.White:
                    for (byte y = 0; y < 3; y++)
                        for (byte x = 0; x < Board.GRID_SIZE; x++)
                            if (y % 2 == x % 2)
                                yield return new Piece(new Point(x, y), PieceType.Man);
                    break;
                case EnumColor.Black:
                    for (byte y = 5; y < Board.GRID_SIZE; y++)
                        for (byte x = 0; x < Board.GRID_SIZE; x++)
                            if (y % 2 == x % 2)
                                yield return new Piece(new Point(x, y), PieceType.Man);
                    break;
            }
        }
        public enum PieceType
        {
            Null = 0,
            Man = 1,
            King = 2
        }

    }
}

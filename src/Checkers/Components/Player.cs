using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Player : IPlayer
    {
        public EnumColor Color { get; set; }
        private IEnumerable<Piece> Pieces { get; set; }
        public Dictionary<Point, Piece.PieceType> PointToPiece { get; private set; } = new Dictionary<Point, Piece.PieceType>();
        public Player ()
        {
        }
        public Player(EnumColor color)
        {
            Color = color;
            Pieces = createPieces(color);
            foreach (var piece in Pieces)
                PointToPiece.Add(piece.Location, piece.TypeOfPiece);
        }
        private IEnumerable<Piece> createPieces(EnumColor color)
        {
            switch (color)
            {
                case EnumColor.White:
                    for (byte y = 0; y < 3; y++)
                        for (byte x = 0; x < 8; x++)
                            if (y % 2 == x % 2)
                                yield return new Piece(new Point(x, y), Piece.PieceType.Man);
                    break;
                case EnumColor.Black:
                    for (byte y = 5; y < 8; y++)
                        for (byte x = 0; x < 8; x++)
                            if (y % 2 == x % 2)
                                yield return new Piece(new Point(x, y), Piece.PieceType.Man);
                    break;
            }
        }
        public enum EnumColor : byte
        {
            White = 1,
            Black = 2
        }
    }
}

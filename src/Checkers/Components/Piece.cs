using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Checkers.Components.Player;

namespace Checkers.Components
{
    public class Piece
    {
        #region Private Constructors

        public Piece(Point point, PieceType type)
        {
            Location = point;
            TypeOfPiece = type;
        }

        #endregion Private Constructors

        #region Internal Enums

        public enum PieceType
        {
            Null = 0,
            Man = 1,
            King = 2
        }

        #endregion Internal Enums

        #region Internal Properties

        internal Point Location { get; }
        internal PieceType TypeOfPiece { get; }

        #endregion Internal Properties

        #region Internal Methods

        internal static IEnumerable<Piece> createPieces(EnumColor color)
        {
            switch (color)
            {
                case EnumColor.White:
                    for (byte y = 0; y < 3; y++)
                    {
                        for (byte x = 0; x < Board.GRID_SIZE; x++)
                        {
                            if (y % 2 == x % 2)
                                yield return new Piece(new Point(x, y), PieceType.Man);
                        }
                    }

                    break;

                case EnumColor.Black:
                    for (byte y = 5; y < Board.GRID_SIZE; y++)
                    {
                        for (byte x = 0; x < Board.GRID_SIZE; x++)
                        {
                            if (y % 2 == x % 2)
                                yield return new Piece(new Point(x, y), PieceType.Man);
                        }
                    }

                    break;
            }
        }

        internal Piece movePiece(byte x, byte y)
        {
            Location.setPoint(x, y);
            return this;
        }

        #endregion Internal Methods
    }
}
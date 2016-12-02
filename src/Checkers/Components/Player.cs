using System.Collections.Generic;
using System.Linq;

namespace Checkers.Components
{
    public class Player
    {
        #region Public Constructors

        internal int Pieces => DictPointPiece.Count;

        #endregion Public Constructors

        #region Public Enums

        public enum EnumColor : byte
        {
            White = 1,
            Black = 2
        }

        #endregion Public Enums

        #region Public Properties

        public EnumColor Color { get; set; }
        public Dictionary<Point, Piece.PieceType> DictPointPiece { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void Init()
        {
            DictPointPiece = Piece.createPieces(Color).ToDictionary(key => key.Location, value => value.TypeOfPiece);
        }

        internal void addPiece(byte x, byte y, Piece.PieceType typePiece)
                    => DictPointPiece.Add(new Point(x, y), typePiece);

        internal void removePiece(byte x, byte y)
        => DictPointPiece.Remove(new Point(x, y));

        #endregion Public Methods
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Player : IPlayer
    {
        public EnumColor Color { get; set; }
        public Dictionary<Point, Piece.PieceType> DictPointPiece { get; private set; }
        public Player()
        {
        }
        public Player(EnumColor color)
        {
            Color = color;
        }
        public void Init()
        {
            DictPointPiece = Piece.createPieces(Color).ToDictionary(key => key.Location, value => value.TypeOfPiece);
        }


        public enum EnumColor : byte
        {
            White = 1,
            Black = 2
        }
        public bool removePiece(byte x, byte y)
        => DictPointPiece.Remove(DictPointPiece.Select(kv => kv.Key).Where(point => point.X == x && point.Y == y).SingleOrDefault());
    }
}

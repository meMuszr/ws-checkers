using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class Board
    {
        public readonly byte GRID_SIZE = 8;
        public byte[,] Grid { get; private set; } = new byte[8, 8];
        public void addToGrid(int x, int y, Player.EnumColor color, Piece.PieceType pieceType)
        {
            if (Grid[y, x] == 0) Grid[y, x] = PieceInfoToByte(color, pieceType);
        }
        private byte PieceInfoToByte(Player.EnumColor color, Piece.PieceType pieceType) => byte.Parse($"{(int)color}{(int)pieceType}");
        public bool isMoveValid(Piece piece, Point newCoordinates)
        {
            if (piece == null) throw new ArgumentNullException();
            switch (piece.TypeOfPiece)
            {
                case Piece.PieceType.Man: return (piece.Location.X + 1 == newCoordinates.X || piece.Location.X - 1 == newCoordinates.X) && piece.Location.Y + 1 == newCoordinates.Y;
                case Piece.PieceType.King:
                    return (piece.Location.X + 1 == newCoordinates.X || piece.Location.X - 1 == newCoordinates.X) &&
                           (piece.Location.Y + 1 == newCoordinates.Y || piece.Location.Y - 1 == newCoordinates.Y);
                default: return false;
            }
        }
        public string PrettyPrint()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    var value = Grid[y, x];
                    if (value == 0) sb.Append("_");
                    else
                    {
                        var stringValue = Grid[y, x].ToString();
                        switch ((Player.EnumColor)((int.Parse(stringValue[0].ToString()))))
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
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private Point isNewCoordinateValid(Point point)
        {
            if (point == null) throw new ArgumentNullException();
            if (point.X >= Grid.Length || point.Y >= Grid.Length) throw new ArgumentException();
            else return point;
        }

        public Tuple<Player.EnumColor, Piece.PieceType> getPieceInfo(int x, int y)
        {
            byte info = Grid[y, x];
            if (info == 0) throw new KeyNotFoundException();
            else
            {
                string data = $"{info}";
                Player.EnumColor color = (Player.EnumColor)int.Parse($"{data[0]}");
                Piece.PieceType pieceType = (Piece.PieceType)int.Parse($"{data[1]}");
                return new Tuple<Player.EnumColor, Piece.PieceType>(color, pieceType);
            }
        }
    }
}

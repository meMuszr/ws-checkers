using System;
using System.Collections.Generic;
namespace Checkers
{
    public class Game
    {
        public Tuple<Player, Player> Players { get; private set; } = new Tuple<Player, Player>(new Player(Player.EnumColor.White), new Player(Player.EnumColor.Black));
        public Board Board { get; private set; } = new Board();
        public Game()
        {
            mapPlayerPiecesToBoard(Players.Item1);
            mapPlayerPiecesToBoard(Players.Item2);
        }
        private void mapPlayerPiecesToBoard(Player player)
        {
            if (player == null) throw new ArgumentNullException();
            var coordinates = player.PointToPiece.Keys;
            foreach (var coordinate in coordinates)
            {
                Piece.PieceType pieceType;
                player.PointToPiece.TryGetValue(coordinate, out pieceType);
                Board.addToGrid(coordinate.X, coordinate.Y, player.Color, pieceType);
            }
        }
    }
    public class Board
    {
        public readonly byte GRID_SIZE = 8;
        public byte[,] Grid { get; private set; } = new byte[8, 8];
        public void addToGrid(int x, int y, Player.EnumColor color, Piece.PieceType pieceType)
        {
            if (Grid[x, y] == 0) Grid[x, y] = PieceInfoToByte(color, pieceType);
        }
        private byte PieceInfoToByte(Player.EnumColor color, Piece.PieceType pieceType) => byte.Parse($"{(int)color}{(int)pieceType}");
        public bool isMoveValid(Piece piece, Point newCoordinates)
        {
            if (piece == null) throw new ArgumentNullException();
            switch(piece.TypeOfPiece)
            {
                case Piece.PieceType.Man:  return (piece.Location.X + 1 == newCoordinates.X || piece.Location.X - 1 == newCoordinates.X) && piece.Location.Y + 1 == newCoordinates.Y;
                case Piece.PieceType.King: return (piece.Location.X + 1 == newCoordinates.X || piece.Location.X - 1 == newCoordinates.X) && 
                                                  (piece.Location.Y + 1 == newCoordinates.Y || piece.Location.Y - 1 == newCoordinates.Y);
                default:  return false;
            }
        }

        private Point isNewCoordinateValid(Point point)
        {
            if (point == null) throw new ArgumentNullException();
            if (point.X >= Grid.Length || point.Y >= Grid.Length) throw new ArgumentException();
            else return point;
        }

        public Tuple<Player.EnumColor, Piece.PieceType> getPieceInfo(int x,int y)
        {
            byte info = Grid[x, y];
            if (info == 0) throw new KeyNotFoundException();
            else
            {
                string data = $"{info}";
                Player.EnumColor color = (Player.EnumColor)int.Parse($"{data[0]}");
                Piece.PieceType pieceType = (Piece.PieceType)int.Parse($"{data[1]}");
                return new Tuple<Player.EnumColor, Piece.PieceType>(color,pieceType);
            }   
        }
    }
    public class Player
    {
        public EnumColor Color { get; private set; }
        private IEnumerable<Piece> Pieces { get; set; }
        public Dictionary<Point, Piece.PieceType> PointToPiece { get; private set; } = new Dictionary<Point, Piece.PieceType>();
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
    public class Point
    {
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
    }

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
        public enum PieceType : byte
        {
            Man = 1, King = 2
        }
    }
}

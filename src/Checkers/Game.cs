using Checkers.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers
{
    public class Game
    {
        public Players<Player, Player> Players { get; private set; } = new Players<Player, Player>();
        public Board Board { get; private set; } = new Board();
        public Game()
        {
            Players.One.Color = Player.EnumColor.White;
            Players.Two.Color = Player.EnumColor.White;
            mapPlayerPiecesToBoard(Players.One);
            mapPlayerPiecesToBoard(Players.Two);
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
  
}

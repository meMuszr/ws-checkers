using Checkers.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers
{
    //Add has another move. map piece removal between the player and the board.
    public class Game
    {
        public Players<Player, Player> Players { get; private set; } = Players<Player, Player>.createPlayers();
        public Board Board { get; private set; } = new Board();
        public Game()
        {
            Players.One.Color = Player.EnumColor.White;
            Players.Two.Color = Player.EnumColor.Black;
            Initalize();

        }
        private void Initalize()
        {
            InitPlayer(Players.One);
            InitPlayer(Players.Two);
        }
        private void InitPlayer(Player player)
        {
            if (player == null) throw new ArgumentNullException();
            player.Init();
            foreach (var coordinate in player.DictPointPiece.Keys)
            {
                Piece.PieceType pieceType;
                player.DictPointPiece.TryGetValue(coordinate, out pieceType);
                Board.addToGrid(coordinate.X, coordinate.Y, player.Color, pieceType);
            }
        }
    }

}

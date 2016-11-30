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
            Initalize();
        }
        private Tuple<Player, Piece> getPieceInfo(int x, int y)
        {
            byte? info = Board.Grid[y, x];
            if (info == null) return null;

            string data = $"{info}";
            Player.EnumColor color = (Player.EnumColor)int.Parse($"{data[0]}");
            Player pieceOfPlayer = Players.getPlayerFromColor(color);

            Piece.PieceType pieceType = (Piece.PieceType)int.Parse($"{data[1]}");
            return new Tuple<Player, Piece>(pieceOfPlayer, new Piece(new Point(x, y), pieceType));
        }
        public Tuple<bool, Piece> checkPiece(Board.Direction direction, Tuple<Player, Piece> oldPieceInfo, Point newCoordinates)
        {
            switch (direction)
            {
                case Board.Direction.NW: break;
                case Board.Direction.NE: break;
                case Board.Direction.SW: break;
                case Board.Direction.SE: break;
            }
            return new Tuple<bool, Piece>(false, null);
        }

        public MoveInfo getMoveInfo(Piece piece, Point newCoordinates)
        {
            if (piece == null) throw new ArgumentNullException();
            if (newCoordinates == null) throw new ArgumentNullException();
            if (newCoordinates.X >= Board.GRID_SIZE || newCoordinates.Y >= Board.GRID_SIZE ||
                newCoordinates.X < 0 || newCoordinates.Y < 0) return new MoveInfo { isValidJumpMove = false, isValidMove = false };

            var rtnMove = new MoveInfo();
            int dX = newCoordinates.X - piece.Location.X,
                dY = newCoordinates.Y - piece.Location.Y;
            Board.Direction directionOfMove = Board.getMoveDirection(dY, dX);

            var isJumpMove = Math.Abs(dX) == 2 && Math.Abs(dY) == 2;
            var isMove = Math.Abs(dX) == 1 && Math.Abs(dY) == 1;

            //ensure they can only move forward if normal, or all directions king
            var pieceInfo = getPieceInfo(newCoordinates.X, newCoordinates.Y);
            var oldPieceInfo = getPieceInfo(piece.Location.X, piece.Location.X);
            if (isMove)
            {
                if (pieceInfo == null)
                {
                    if (oldPieceInfo.Item1.Color == Player.EnumColor.White)
                    {
                        if (piece.TypeOfPiece == Piece.PieceType.Man)
                            rtnMove.isValidMove = directionOfMove == Board.Direction.NE || directionOfMove == Board.Direction.NW;
                    }
                    else
                    {
                        if (piece.TypeOfPiece == Piece.PieceType.Man)
                            rtnMove.isValidMove = directionOfMove == Board.Direction.SE || directionOfMove == Board.Direction.SW;
                    }
                }
                else rtnMove.isValidMove = false;
            }
            else if (isJumpMove)
            {
                if (oldPieceInfo.Item1.Color == Player.EnumColor.White)
                {
                    if (piece.TypeOfPiece == Piece.PieceType.Man)
                        rtnMove.isValidMove = directionOfMove == Board.Direction.NE || directionOfMove == Board.Direction.NW;
                }
                else
                {
                    if (piece.TypeOfPiece == Piece.PieceType.Man)
                        rtnMove.isValidMove = directionOfMove == Board.Direction.SE || directionOfMove == Board.Direction.SW;
                }
                rtnMove.isValidJumpMove = Board.checkPiece(directionOfMove, oldPieceInfo, newCoordinates);

            }
            else return new MoveInfo { isValidJumpMove = false, isValidMove = false };

            return rtnMove;

        }

        private void Initalize()
        {
            Players.One.Color = Player.EnumColor.White;
            Players.Two.Color = Player.EnumColor.Black;

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

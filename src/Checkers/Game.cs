using Checkers.Components;
using System;

namespace Checkers
{
    //Add has another move.
    public class Game
    {
        #region Public Constructors

        public Game()
        {
            Initalize();
        }

        #endregion Public Constructors

        #region Public Enums

        public enum StateOfGame
        {
            White = 1, Black = 2, GameOver = 3
        }

        #endregion Public Enums

        #region Public Properties

        public Board Board { get; } = new Board();
        public Players<Player, Player> Players { get; } = Players<Player, Player>.createPlayers();
        public StateOfGame State { get; set; }

        #endregion Public Properties

        #region Public Methods

        public MoveValidation MakeMove(Point point, Point newPoint)
        {
            if (point == null || newPoint == null) throw new ArgumentNullException();
            var rtnValue = default(MoveValidation);
            Tuple<Player, Piece> oldPiece = getPieceInfo(point.X, point.Y);

            if (oldPiece == null) throw new ArgumentException();
            MoveInfo moveInfo = getMoveInfo(oldPiece.Item2, newPoint);
            if (State == StateOfGame.White && oldPiece.Item1.Color == Player.EnumColor.White || State == StateOfGame.Black && oldPiece.Item1.Color == Player.EnumColor.Black)
            {
                if (moveInfo.isValidMove || moveInfo.isValidJumpMove)
                {
                    rtnValue = new MoveValidation
                    {
                        UpdateMove = true,
                        OldPoint = point,
                        NewPoint = newPoint
                    };
                    oldPiece.Item1.removePiece(point.X, point.Y);
                    oldPiece.Item1.addPiece(newPoint.X, newPoint.Y, oldPiece.Item2.TypeOfPiece);
                    Board.relocatePoint(point, newPoint);
                    if (moveInfo.JumpedPiece != null && moveInfo.isValidJumpMove)
                    {
                        rtnValue.RemovePoint = moveInfo.JumpedPiece.Location;
                        getPieceInfo(moveInfo.JumpedPiece.Location.X, moveInfo.JumpedPiece.Location.Y).Item1.removePiece(moveInfo.JumpedPiece.Location.X, moveInfo.JumpedPiece.Location.Y);
                        Board.removePoint(moveInfo.JumpedPiece.Location);
                    }
                    State = State == StateOfGame.White ? StateOfGame.Black : StateOfGame.White;
                    rtnValue.GameState = State;
                }

            }
            if (Players.One.Pieces == 0 || Players.Two.Pieces == 0)
                State = StateOfGame.GameOver;
            return rtnValue ?? new MoveValidation();
        }

        #endregion Public Methods

        #region Private Methods

        private MoveInfo getMoveInfo(Piece piece, Point newCoordinates)
        {
            if (piece == null) throw new ArgumentNullException();
            if (newCoordinates == null) throw new ArgumentNullException();
            var rtnMove = new MoveInfo();

            if (newCoordinates.X >= Board.GRID_SIZE || newCoordinates.Y >= Board.GRID_SIZE ||
                newCoordinates.X < 0 || newCoordinates.Y < 0)
            {
                return rtnMove;
            }

            int dX = newCoordinates.X - piece.Location.X,
                dY = newCoordinates.Y - piece.Location.Y;

            Board.Direction directionOfMove = Board.getMoveDirection(dY, dX);

            bool isJumpMove = Math.Abs(dX) == 2 && Math.Abs(dY) == 2;
            bool isMove = Math.Abs(dX) == 1 && Math.Abs(dY) == 1;

            Tuple<Player, Piece> pieceInfo = getPieceInfo(newCoordinates.X, newCoordinates.Y);
            Tuple<Player, Piece> oldPieceInfo = getPieceInfo(piece.Location.X, piece.Location.Y);

            if (pieceInfo == null)
            {
                if (isMove)
                {
                    if (oldPieceInfo.Item1.Color == Player.EnumColor.White)
                    {
                        if (piece.TypeOfPiece == Piece.PieceType.Man)
                            rtnMove.isValidMove = directionOfMove == Board.Direction.SE || directionOfMove == Board.Direction.SW;
                    }
                    else
                    {
                        if (piece.TypeOfPiece == Piece.PieceType.Man)
                            rtnMove.isValidMove = directionOfMove == Board.Direction.NE || directionOfMove == Board.Direction.NW;
                    }
                }
                else if (isJumpMove)
                {
                    bool validDirection = true;
                    if (oldPieceInfo.Item1.Color == Player.EnumColor.White)
                    {
                        if (piece.TypeOfPiece == Piece.PieceType.Man)
                            validDirection = directionOfMove == Board.Direction.SE || directionOfMove == Board.Direction.SW;
                    }
                    else
                    {
                        if (piece.TypeOfPiece == Piece.PieceType.Man)
                            validDirection = directionOfMove == Board.Direction.NE || directionOfMove == Board.Direction.NW;
                    }
                    if (validDirection)
                    {
                        Tuple<bool, Piece> validate = validateJumpMove(directionOfMove, oldPieceInfo, newCoordinates);
                        rtnMove.isValidJumpMove = validate.Item1;
                        rtnMove.JumpedPiece = validate.Item2;
                    }
                }
            }
            return rtnMove;
        }

        private Tuple<Player, Piece> getPieceInfo(int x, int y)
        {
            byte? info = Board.Grid[y, x];
            if (info == null) return null;

            string data = $"{info}";
            var color = (Player.EnumColor)int.Parse($"{data[0]}");
            Player pieceOfPlayer = Players.getPlayerFromColor(color);

            var pieceType = (Piece.PieceType)int.Parse($"{data[1]}");
            return new Tuple<Player, Piece>(pieceOfPlayer, new Piece(new Point(x, y), pieceType));
        }

        private void Initalize()
        {
            Players.One.Color = Player.EnumColor.White;
            Players.Two.Color = Player.EnumColor.Black;
            State = StateOfGame.White;
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

        private Tuple<bool, Piece> validateJumpMove(
            Board.Direction directionOfMove,
            Tuple<Player, Piece> oldPieceInfo,
            Point newCoordinates)
        {
            Tuple<Player, Piece> pieceInfo;
            Tuple<bool, Piece> rtnInfo = null;
            switch (directionOfMove)
            {
                case Board.Direction.NE: // (0,4) -> (1,3)
                    pieceInfo = getPieceInfo(newCoordinates.X - 1, newCoordinates.Y - 1);
                    if (pieceInfo?.Item1.Color != oldPieceInfo?.Item1.Color)
                        rtnInfo = new Tuple<bool, Piece>(true, pieceInfo?.Item2);
                    break;

                case Board.Direction.NW:// (7,4) -> (6,3)
                    pieceInfo = getPieceInfo(newCoordinates.X + 1, newCoordinates.Y - 1);
                    if (pieceInfo?.Item1.Color != oldPieceInfo?.Item1.Color)
                        rtnInfo = new Tuple<bool, Piece>(true, pieceInfo.Item2);
                    break;

                case Board.Direction.SW:
                    pieceInfo = getPieceInfo(newCoordinates.X + 1, newCoordinates.Y + 1);
                    if (pieceInfo?.Item1.Color != oldPieceInfo?.Item1.Color)
                        rtnInfo = new Tuple<bool, Piece>(true, pieceInfo.Item2);
                    break;

                case Board.Direction.SE:
                    pieceInfo = getPieceInfo(newCoordinates.X - 1, newCoordinates.Y + 1);
                    if (pieceInfo?.Item1.Color != oldPieceInfo?.Item1.Color)
                        rtnInfo = new Tuple<bool, Piece>(true, pieceInfo.Item2);
                    break;
            }
            return rtnInfo;
        }

        #endregion Private Methods
    }
}
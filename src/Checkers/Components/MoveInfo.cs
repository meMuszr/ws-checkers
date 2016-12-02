namespace Checkers.Components
{
    internal class MoveInfo
    {
        #region Public Properties

        internal bool canJumpAgain { get; set; }
        internal bool isValidJumpMove { get; set; }
        internal bool isValidMove { get; set; }
        internal Piece JumpedPiece { get; set; }

        #endregion Public Properties
    }
}
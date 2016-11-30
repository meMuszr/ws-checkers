using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
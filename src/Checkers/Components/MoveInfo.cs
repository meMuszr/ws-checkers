using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public class MoveInfo
    {
        public bool isValidJumpMove { get; set; }
        public bool canJumpAgain { get; set; }
        public bool isValidMove { get; set; }
        public Piece JumpedPiece { get; set; }

    }
}

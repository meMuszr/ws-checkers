﻿using Checkers;
using Xunit;

namespace CheckersTests
{
    public class LogicTests
    {
        [Fact]
        public void PassingTest()
        {
            var game = new Game();
            var outstr = game.Board.PrettyPrint();

        }

    }
}

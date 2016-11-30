using Checkers;
using Checkers.Components;
using Xunit;

namespace CheckersTests
{
    public class LogicTests
    {
        #region Public Methods

        [Fact]
        public void PassingTest()
        {
            var game = new Game();
            game.MakeMove(new Point(0, 2), new Point(1, 3));
            game.MakeMove(new Point(1, 5), new Point(0, 4));
            game.MakeMove(new Point(2, 2), new Point(3, 3));
            game.MakeMove(new Point(0, 4), new Point(2, 2));

            var outstr = game.Board.ToString();
        }

        #endregion Public Methods
    }
}
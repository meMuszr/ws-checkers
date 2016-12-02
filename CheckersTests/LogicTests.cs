using Checkers;
using Checkers.Components;
using System;
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
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(1, 5), new Point(0, 4));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(2, 2), new Point(3, 3));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(0, 4), new Point(2, 2));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");

            var outstr = game.Board.ToString();
        }

        #endregion Public Methods
    }
}
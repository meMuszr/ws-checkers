using Checkers;
using Checkers.Components;
using CheckersWS.WebSocket.Models;
using System;
using Xunit;

namespace CheckersTests
{
    public class LogicTests
    {
        #region Public Methods

        [Fact]
        public void CheckDataTransmission()
        {
            var data = new DataTransmission<MoveValidation> { Message = new MoveValidation { GameState = Game.StateOfGame.GameOver, UpdateMove = true } };
            Console.WriteLine(data.Message);
        }

        [Fact]
        public void PassingTest()
        {
            var game = new Game();
            game.MakeMove(new Point(0, 5), new Point(1, 4));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(3, 2), new Point(2, 3));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(1, 4), new Point(3, 2));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(1, 2), new Point(2, 3));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(2, 5), new Point(1, 4));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(2, 3), new Point(0, 5));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(6, 5), new Point(7, 4));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            game.MakeMove(new Point(5, 2), new Point(6, 3));
            Console.WriteLine(game.Board.ToString());
            Console.WriteLine("             ");
            Console.WriteLine("             ");
            var outstr = game.Board.ToString();
        }

        #endregion Public Methods
    }
}
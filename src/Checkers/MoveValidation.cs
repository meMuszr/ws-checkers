using Checkers.Components;

namespace Checkers
{
    public class MoveValidation
    {
       public bool UpdateMove { get; set; }
       public Point OldPoint { get; set; }
       public Point NewPoint { get; set; }
       public Point RemovePoint { get; set; }
       public Game.StateOfGame GameState { get; set; }
    }
}
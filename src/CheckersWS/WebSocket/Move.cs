using Checkers.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckersWS.WebSocket
{
    public class Move
    {
        public Point OldPoint { get;set;}
        public Point NewPoint { get; set; }
    }
}

using Checkers;
using System.Collections.Generic;

namespace CheckersWS.WebSocket
{
    public class Games
    {
        public Dictionary<string,Handler> Users { get; } = new Dictionary<string, Handler>();
        public Game GameInstance { get; }

        public Games()
        {
            GameInstance = new Game();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckersWS.WebSocket
{
    public class User
    {
        public string Name { get; set; }
        public IEnumerable<Games> Games { get; set; }

        public User(string name)
        {
            Name = name;
        }
    }
}

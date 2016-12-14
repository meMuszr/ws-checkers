using System.Collections.Generic;

namespace CheckersWS.WebSocket
{
    public class User
    {
        #region Public Constructors

        public User(string name)
        {
            Name = name;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool isInGame { get; set; }
        public string Name { get; set; }

        #endregion Public Properties
    }
}
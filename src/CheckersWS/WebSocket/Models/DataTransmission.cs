using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckersWS.WebSocket.Models
{
    public class DataTransmission<T>
    {
        #region Public Properties

        public T Message { get; set; }
        public string MessageType { get; set; }

        #endregion Public Properties
    }
}
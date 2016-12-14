using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckersWS.WebSocket.Models
{
    public class DataTransmission<T>
    {
        public string MessageType { get; set; }

        public T Message {get;set;}
    }
}

using CheckersWS.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WS = System.Net.WebSockets;

namespace CheckersWS
{
    public class Handler
    {
        #region Private Fields

        private static readonly IEqualityComparer<User> UserComparer = new UsersComparer();
        private static HashSet<User> Users = new HashSet<User>(comparer: UserComparer);
        private WS.WebSocket Socket;

        #endregion Private Fields

        #region Private Constructors

        private Handler(WS.WebSocket socket)
        {
            Socket = socket;
        }

        #endregion Private Constructors

        #region Private Properties

        private static List<WS.WebSocket> _webSockets { get; } = new List<WS.WebSocket>();
        private User User { get; set; }

        #endregion Private Properties

        #region Public Methods

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(AcceptorAsync);
        }

        #endregion Public Methods

        #region Private Methods

        private static async Task AcceptorAsync(HttpContext context, Func<Task> n)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;

            using (WS.WebSocket socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                _webSockets.Add(socket);
                var handler = new Handler(socket);
                await handler.EchoLoopAsync();
            }
        }

        private async Task EchoLoopAsync()
        {
            var buffer = new byte[1024 * 4];
            while (Socket.State == WS.WebSocketState.Open)
            {
                var incoming = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (incoming.MessageType == WS.WebSocketMessageType.Text)
                {
                    var hello = new ArraySegment<byte>(buffer);
                    string[] data = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, incoming.Count).ToArray()).Split(':');
                    switch (data[0])
                    {
                        case "login":
                            User = new User(data[1]);
                            if (!Users.Contains(User))
                            {
                                Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"login:{JsonConvert.SerializeObject(Users.Select(c => c.Name))}")), WS.WebSocketMessageType.Text, incoming.EndOfMessage, CancellationToken.None);
                                Users.Add(User);
                            }
                            _webSockets.ForEach(webSocket => webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"login:{JsonConvert.SerializeObject(data[1])}")), WS.WebSocketMessageType.Text, incoming.EndOfMessage, CancellationToken.None));
                            Console.WriteLine(data[1]);

                            break;
                    }
                }
            }
            if (Socket.State != WS.WebSocketState.Open)
            {
                _webSockets.Remove(Socket);
                Users.Remove(User);
                _webSockets.ForEach(webSocket => webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"removelogin:{JsonConvert.SerializeObject(User.Name)}")), WS.WebSocketMessageType.Text, true, CancellationToken.None));
            }
        }

        #endregion Private Methods

        #region Private Classes

        private class UsersComparer : IEqualityComparer<User>
        {
            #region Public Methods

            public bool Equals(User x, User y)
            {
                if (x == null && y == null)
                    return true;
                else if (x == null | y == null)
                    return false;
                else return x.Name == y.Name;
            }

            public int GetHashCode(User obj)
            => (obj == null) ? 0 : obj.Name.GetHashCode();

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}
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
        private static readonly IEqualityComparer<User> UserComparer = new UsersComparer();

        private static HashSet<User> Users = new HashSet<User>(comparer: UserComparer);
        private readonly WS.WebSocket Socket;

        private Handler(WS.WebSocket socket)
        {
            Socket = socket;
        }

        private static List<Handler> _handler { get; } = new List<Handler>();
        internal User User { get; set; }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(AcceptorAsync);
        }

        private static async Task AcceptorAsync(HttpContext context, Func<Task> n)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;

            using (WS.WebSocket socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                var handler = new Handler(socket);
                _handler.Add(handler);
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
                            _handler.ForEach(handler => handler.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"login:{JsonConvert.SerializeObject(data[1])}")), WS.WebSocketMessageType.Text, incoming.EndOfMessage, CancellationToken.None));
                            Console.WriteLine(data[1]);
                            break;
                        case "newGame":
                            User Opponent = new User(data[1]);
                            if (Users.Contains(Opponent))
                            {
                                var game = _handler.Where(c => c.User.Name == User.Name || c.User.Name == Opponent.Name).ToList();
                                if (game.Count == 2) {
                                    game.ForEach(handler => {
                                    });
                                }
                            }   break;
                    }
                }
            }
            if (Socket.State != WS.WebSocketState.Open)
            {
                _handler.Remove(this);
                Users.Remove(User);
                _handler.ForEach(handler => handler.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"removelogin:{JsonConvert.SerializeObject(User.Name)}")), WS.WebSocketMessageType.Text, true, CancellationToken.None));
            }
        }

        private class UsersComparer : IEqualityComparer<User>
        {
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
        }
    }
}
using WS = System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using CheckersWS.WebSocket;
using System.Linq;
using Newtonsoft.Json;
namespace CheckersWS
{
    public class Handler
    {
        private static List<WS.WebSocket> _webSockets { get; set; } = new List<WS.WebSocket>();
        private static HashSet<User> Users = new HashSet<User>();
        private WS.WebSocket Socket;
        private User User { get; set; }

        Handler(WS.WebSocket socket)
        {
            Socket = socket;
        }

        private static async Task Acceptor(HttpContext context, Func<Task> n)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;

            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                _webSockets.Add(socket);
                var handler = new Handler(socket);
                await handler.EchoLoop();
            }
        }
        private async Task EchoLoop()
        {
            var buffer = new byte[1024 * 4];
            while (Socket.State == WS.WebSocketState.Open)
            {
                var incoming = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (incoming.MessageType == WS.WebSocketMessageType.Text)
                {
                    var hello = new ArraySegment<byte>(buffer);
                    var data = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer,0,incoming.Count).ToArray()).Split(':');
                    switch (data[0])
                    {
                        case "login":
                            if (!Users.Contains(new User(data[1]))) Users.Add(new User(data[1]));
                            Console.WriteLine(data[1]);
                            break;
                    }
                }
                    var getUsers = Encoding.UTF8.GetBytes($"login:{JsonConvert.SerializeObject(Users.Select(c => c.Name))}");
                _webSockets.ForEach(webSocket => webSocket.SendAsync(new ArraySegment<byte>(getUsers), WS.WebSocketMessageType.Text, incoming.EndOfMessage, CancellationToken.None));
            }
        }
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(Acceptor);
        }
    }
}

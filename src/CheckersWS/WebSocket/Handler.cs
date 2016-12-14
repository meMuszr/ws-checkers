using CheckersWS.WebSocket;
using CheckersWS.WebSocket.Models;
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

        private static HashSet<User> Users = new HashSet<User>(UserComparer);
        private readonly WS.WebSocket Socket;

        private Handler(WS.WebSocket socket)
        {
            Socket = socket;
        }

        private static List<Handler> _handler { get; } = new List<Handler>();
        private static Dictionary<int, Games> DictGames { get; } = new Dictionary<int, Games>();
        private static int Count { get; set; }
        internal User User { get; set; }
        internal int? GameId { get; set; }
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
            var existingLogin = false;
            while (Socket.State == WS.WebSocketState.Open)
            {
                //save incoming data
                var incoming = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (incoming.MessageType == WS.WebSocketMessageType.Text)
                {
                    //parse incoming data
                    var data = ParseIncomingData<string>(incoming);
                    //holding variable for out method
                    Games currentGame = default(Games);
                    //switch messageTypes
                    switch (data.MessageType)
                    {
                        case "login":
                            //Create User with this username
                            User = new User(data.Message);
                            //If not included in hashset,
                            if (!Users.Contains(User))
                            {
                                //send everybody to this user
                                await SendCompleteMessageAsync(Socket, new DataTransmission<IEnumerable<string>> { MessageType = "login", Message = Users.Where(c => c.isInGame == false).Select(c => c.Name) });
                                //add user to hashset
                                Users.Add(User);
                            }
                            //if in hashset
                            else
                            {
                                //close connection
                                await Socket.CloseAsync(WS.WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                                //let server know that this handler is an existinglogin
                                existingLogin = true;
                                break;
                            }
                            //send user to everybody
                            _handler.ForEach(async handler => await SendCompleteMessageAsync(handler.Socket, new DataTransmission<string> { MessageType = "login", Message = data.Message}));
                            break;
                        case "newGame":
                            //Create opponent with recieved username
                            User Opponent = new User(data.Message);
                            //validation checking,
                            if (Users.Contains(Opponent))
                            {
                                //find handlers where username is equal to requested opponent and sender's username -- update IsInGame status to true, convert is true
                                var game = _handler.Where(c => c.User.Name == User.Name || c.User.Name == Opponent.Name).Select(c => { c.User.isInGame = true; return c; }).ToList();
                                //More validation checking, we should only recieve 2 entries
                                if (game.Count == 2)
                                {
                                    //tell everyone in lobby to remove people who just made game
                                    _handler.Where(c => c.User.isInGame == false).ToList().ForEach(
                                        async handler => await SendCompleteMessageAsync(handler.Socket, new DataTransmission<string[]> { MessageType = "removelogin", Message = new string[2] { User.Name, Opponent.Name } }));
                                    //add to dictionary
                                    DictGames.Add(Count, new Games());
                                    //try get game value
                                    DictGames.TryGetValue(Count, out currentGame);
                                    if (currentGame != null) {
                                        game.ForEach(handler =>
                                        {
                                            //set gameID for handler 
                                            handler.GameId = Count;
                                            //
                                            DictGames[Count].Users.Add(handler.User.Name, handler);
                                        });

                                       foreach(var user in currentGame.Users)
                                        {
                                            //give them
                                            await SendCompleteMessageAsync(user.Value.Socket, new DataTransmission<Dictionary<string,Handler>.KeyCollection> { MessageType = "createGame", Message = DictGames[Count].Users.Keys });
                                            await SendCompleteMessageAsync(user.Value.Socket, new DataTransmission<int> { MessageType = "gameId", Message = Count });
                                        };
                                    }
                                    await game.Find(c => c.User.Name == User.Name).Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"isTurn={JsonConvert.SerializeObject(new {isTurn = true, Color = "White"})} ")), WS.WebSocketMessageType.Text, true, CancellationToken.None);
                                    await game.Find(c => c.User.Name == Opponent.Name).Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"isTurn={JsonConvert.SerializeObject(new {isTurn = false, Color = "Black"})} ")), WS.WebSocketMessageType.Text, true, CancellationToken.None);
                                    Count++;
                                }
                            }
                            break;
                        case "makeMove":
                          var moveData =  JsonConvert.DeserializeObject<Move>(data.Message);
                            DictGames.TryGetValue(GameId ?? -1, out currentGame);
                           var moveInfo = currentGame.GameInstance.MakeMove(moveData.OldPoint, moveData.NewPoint);
                            foreach(var user in currentGame.Users)
                            await user.Value.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"move={JsonConvert.SerializeObject(moveInfo)}")), WS.WebSocketMessageType.Text, true, CancellationToken.None);


                            break;
                    }

                }
            }
            if (Socket.State != WS.WebSocketState.Open)
            {
                //remove from handler
                _handler.Remove(this);
                if (!existingLogin)
                {
                    Users.Remove(User);
                    if (GameId.HasValue)
                    {
                        Games game = default(Games);
                        DictGames.TryGetValue((int)GameId, out game);
                        if (game != null)
                        {
                            _handler.Where(c => game.Users.ContainsKey(c.User.Name)).Select(c => { c.User.isInGame = false; return c; }).ToList().ForEach(handler =>
                                handler.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"removeGame={JsonConvert.SerializeObject(GameId)}")), WS.WebSocketMessageType.Text, true, CancellationToken.None));
                            DictGames.Remove((int)GameId);
                        }


                    }
                    _handler.ForEach(handler => handler.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"removelogin={JsonConvert.SerializeObject(User.Name)}")), WS.WebSocketMessageType.Text, true, CancellationToken.None));
                }
            }
        }
        private static async Task SendCompleteMessageAsync<T>(WS.WebSocket Socket, DataTransmission<T> data) {
        await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"{data.MessageType}={JsonConvert.SerializeObject(data.Message)}")), WS.WebSocketMessageType.Text, true, CancellationToken.None);
        }
        private static DataTransmission<T> ParseIncomingData<T>(WS.WebSocketReceiveResult retrival)
        {
            var buffer = new byte[1024 * 4];
            string[] data = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, retrival.Count).ToArray()).Split('=');
            return new DataTransmission<T>
            {
                MessageType = data[0],
                Message = JsonConvert.DeserializeObject<T>(data[1])
            };
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
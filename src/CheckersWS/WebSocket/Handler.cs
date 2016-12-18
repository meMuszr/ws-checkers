using Checkers;
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
        #region Private Fields

        private static readonly IEqualityComparer<User> UserComparer = new UsersComparer();

        private static HashSet<User> Users = new HashSet<User>(UserComparer);
        private readonly WS.WebSocket Socket;

        #endregion Private Fields

        #region Private Constructors

        private Handler(WS.WebSocket socket)
        {
            Socket = socket;
        }

        #endregion Private Constructors

        #region Internal Properties

        internal int? GameId { get; set; }
        internal User User { get; set; }

        #endregion Internal Properties

        #region Private Properties

        private static List<Handler> _handler { get; } = new List<Handler>();
        private static int Count { get; set; }
        private static Dictionary<int, Games> DictGames { get; } = new Dictionary<int, Games>();

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
                var handler = new Handler(socket);
                _handler.Add(handler);
                await handler.EchoLoopAsync();
            }
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

        //wrapper method for obnoxious data send
        private static async Task SendCompleteMessageAsync<T>(WS.WebSocket Socket, DataTransmission<T> data)
        {
            await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"{data.MessageType}={JsonConvert.SerializeObject(data.Message)}")), WS.WebSocketMessageType.Text, true, CancellationToken.None);
        }

        //where connection stays alive
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
                    string[] dataString = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, incoming.Count).ToArray()).Split('=');
                    //parse incoming data
                    var data = new DataTransmission<string> { Message = dataString[1], MessageType = dataString[0] };
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
                            _handler.ForEach(async handler => await SendCompleteMessageAsync(handler.Socket, new DataTransmission<string> { MessageType = "login", Message = data.Message }));
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
                                    if (currentGame != null)
                                    {
                                        game.ForEach(handler =>
                                        {
                                            //set gameID for handler
                                            handler.GameId = Count;
                                            //add users to game's dictionary of users
                                            DictGames[Count].Users.Add(handler.User.Name, handler);
                                        });

                                        foreach (var user in currentGame.Users)
                                        {
                                            //give them acknowledgement that game is good to go
                                            await SendCompleteMessageAsync(user.Value.Socket, new DataTransmission<Dictionary<string, Handler>.KeyCollection> { MessageType = "createGame", Message = DictGames[Count].Users.Keys });
                                            //give them gameId to send back with each message
                                            await SendCompleteMessageAsync(user.Value.Socket, new DataTransmission<int> { MessageType = "gameId", Message = Count });
                                        };
                                    }
                                    //assign person who requested a game white / opponent black
                                    await game.Find(c => c.User.Name == User.Name).Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"isTurn={JsonConvert.SerializeObject(new { isTurn = true, Color = "White" })} ")), WS.WebSocketMessageType.Text, true, CancellationToken.None);
                                    await game.Find(c => c.User.Name == Opponent.Name).Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"isTurn={JsonConvert.SerializeObject(new { isTurn = false, Color = "Black" })} ")), WS.WebSocketMessageType.Text, true, CancellationToken.None);
                                    Count++;
                                }
                            }
                            break;

                        case "makeMove":
                            //parse data into move object
                            var moveData = JsonConvert.DeserializeObject<Move>(data.Message);
                            //get game instance from gameId
                            DictGames.TryGetValue(GameId ?? -1, out currentGame);
                            //request game to make move
                            var moveInfo = currentGame.GameInstance.MakeMove(moveData.OldPoint, moveData.NewPoint);
                            foreach (var user in currentGame.Users)
                                //send move info to game users
                                await SendCompleteMessageAsync(user.Value.Socket, new DataTransmission<MoveValidation> { MessageType = "move", Message = moveInfo });
                            //if no pieces left on board / aka game over
                            if (moveInfo.GameState == Game.StateOfGame.GameOver)
                            {
                                //get usernames from that game
                                Dictionary<string, Handler>.KeyCollection currentGameUsers = currentGame.Users.Keys;
                                //find handlers that were part of game, and reset their ingame status
                                _handler.Where(c => currentGameUsers.Contains(c.User.Name)).Select(c => { c.User.isInGame = false; return c; });
                                //send to game users that their game is over and they should reset to lobby
                                foreach (var user in currentGame.Users) await SendCompleteMessageAsync(user.Value.Socket, new DataTransmission<bool> { MessageType = "reset", Message = true });
                                //send to everybody a (fake) login of everybody
                                _handler.ForEach(async handler
                                    => await SendCompleteMessageAsync(handler.Socket, new DataTransmission<IEnumerable<string>> { MessageType = "login", Message = Users.Where(c => !c.isInGame).Select(c => c.Name) }));
                            }
                            break;
                    }
                }
            }
            if (Socket.State != WS.WebSocketState.Open)
            {
                //remove from handler
                _handler.Remove(this);
                //if it wasnt an existinglogin attempt
                if (!existingLogin)
                {
                    //remove from users list
                    Users.Remove(User);
                    //if they were in game when d/c'd
                    if (GameId.HasValue)
                    {
                        //get game instance
                        var game = default(Games);
                        DictGames.TryGetValue((int)GameId, out game);
                        //if game isn't null / we found something
                        if (game != null)
                        {
                            //get those users
                            Dictionary<string, Handler>.KeyCollection currentGameUsers = game.Users.Keys;
                            //get the user that doesnt contain the d/c'd username, and set their ingame status to false; (should only be one)
                            var gameUser = _handler.Where(c => !currentGameUsers.Contains(c.User.Name)).Select(c => { c.User.isInGame = false; return c; }).FirstOrDefault();
                            //send that user a reset msg
                            await SendCompleteMessageAsync(gameUser.Socket, new DataTransmission<bool> { MessageType = "reset", Message = true });
                            //remove game from dictionary
                            DictGames.Remove((int)GameId);
                        }
                    }
                    //remove the login name from everybodys list
                    _handler.ForEach(async handler => await SendCompleteMessageAsync(handler.Socket, new DataTransmission<string> { MessageType = "removelogin", Message = User.Name }));
                }
            }
        }

        #endregion Private Methods

        #region Private Classes
        //implementation of IEqualityComparere for User obj.
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
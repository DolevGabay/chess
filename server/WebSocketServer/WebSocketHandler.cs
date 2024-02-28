using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WebSocketHandler
{
    public async Task HandleWebSocket(WebSocket socket, GameEngine gameEngine, Game game, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        try
        {
            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message: {message}");
                    JObject jsonObject = JObject.Parse(message);

                    if(jsonObject["action"].ToString() == "availableMoves")
                    {
                        int row = jsonObject["row"].Value<int>(); // Extract the integer value
                        int col = jsonObject["col"].Value<int>(); // Extract the integer value
                        game.handleAvailableMoves(row, col);
                    } 
                    else if(jsonObject["action"].ToString() == "move")
                    {
                        int fromRow = jsonObject["fromRow"].Value<int>(); // Extract the integer value
                        int fromCol = jsonObject["fromCol"].Value<int>(); // Extract the integer value
                        int toRow = jsonObject["toRow"].Value<int>(); // Extract the integer value
                        int toCol = jsonObject["toCol"].Value<int>(); // Extract the integer value
                        game.handleMove(fromRow, fromCol, toRow, toCol);
                    }
                    else if(jsonObject["action"].ToString() == "signup")
                    {
                        gameEngine.handleSignup(socket, jsonObject["username"].ToString(), jsonObject["password"].ToString());
                    }
                    else if(jsonObject["action"].ToString() == "signin")
                    {
                        gameEngine.handleSignin(socket, jsonObject["username"].ToString(), jsonObject["password"].ToString());
                    }
                }
            } while (!result.CloseStatus.HasValue && !cancellationToken.IsCancellationRequested);

            if (!cancellationToken.IsCancellationRequested)
            {
                gameEngine.removeGame(game.getGameId());
                game.removePlayer(socket);
                Console.WriteLine("WebSocket closed");
            }
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            Console.WriteLine("The remote party closed the WebSocket connection without completing the close handshake.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("WebSocket operation canceled.");
        }
    }
}

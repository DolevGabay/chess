using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketHandler
{
    public async Task HandleWebSocket(WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        Board board = new Board();
        Guid boardId = board.getBoardId();
        string[,] boardState = board.getBoardToClient();

        //print board
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Console.Write(boardState[i, j] + " ");
            }
            Console.WriteLine();
        }
        
        // Convert multi-dimensional array to jagged array
        string[][] jaggedBoardState = ConvertToJaggedArray(boardState);
        
        // Create an anonymous object to represent the board info
        var boardInfo = new
        {
            action = "newGame",
            boardId = boardId,
            boardState = jaggedBoardState
        };

        // Serialize the anonymous object to JSON using System.Text.Json
        var options = new JsonSerializerOptions { WriteIndented = true };
        var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);

        // Send the serialized JSON data to the client
        await socket.SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        do
        {
            result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message: {message}");

                // Echo the message back to the client
                await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        } while (!result.CloseStatus.HasValue);

        await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
    
    // Method to convert multi-dimensional array to jagged array
    private string[][] ConvertToJaggedArray(string[,] multiDimensionalArray)
    {
        int rows = multiDimensionalArray.GetLength(0);
        int cols = multiDimensionalArray.GetLength(1);

        string[][] jaggedArray = new string[rows][];
        for (int i = 0; i < rows; i++)
        {
            jaggedArray[i] = new string[cols];
            for (int j = 0; j < cols; j++)
            {
                jaggedArray[i][j] = multiDimensionalArray[i, j];
            }
        }
        return jaggedArray;
    }
}

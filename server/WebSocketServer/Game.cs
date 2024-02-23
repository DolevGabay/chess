using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System;
using System.Text.Json;
using System.Text;

public class Game{
    private Guid gameId;
    private Board board;
    private Player player1;
    private Player player2;
    bool homeTurn = true;
    private int turnCount = 0;

    public Game(Player player)
    {
        gameId = Guid.NewGuid();
        player1 = player; 
        handleStartGame();
    }

    public void addPlayer(Player player)
    {
        player2 = player;
        player2.setHome(false);
        Console.WriteLine("Player added to game");
        handleJoinGame();
    }

    public async void handleStartGame()
    {
        board = new Board();
        string[,] boardState = board.getBoardToClient();

        Guid playerId = player1.getPlayerId();

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
            gameId = gameId,
            playerId = playerId,
            boardState = jaggedBoardState,
            home = true,
            gameStarted = false
        };

        // Serialize the anonymous object to JSON using System.Text.Json
        var options = new JsonSerializerOptions { WriteIndented = true };
        var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);

        // Send the serialized JSON data to the client
        await player1.getPlayerSocket().SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine("Game started");
    }

    public async void handleJoinGame()
    {
        string[,] boardState = board.getBoardToClient();
        string[][] jaggedBoardState = ConvertToJaggedArray(boardState);
        Guid playerId = player2.getPlayerId();

        var boardInfo = new
        {
            action = "newGame",
            gameId = gameId,
            playerId = playerId,
            boardState = jaggedBoardState,
            home = false,
            gameStarted = true
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);

        // Send the serialized JSON data to the client
        await player2.getPlayerSocket().SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        
        var notifyPlayer1 = new
        {
            action = "gameStarted",
            gameStarted = true
        };

        var notifyPlayer1Bytes = JsonSerializer.SerializeToUtf8Bytes(notifyPlayer1, options);
        await player1.getPlayerSocket().SendAsync(new ArraySegment<byte>(notifyPlayer1Bytes), WebSocketMessageType.Text, true, CancellationToken.None);

        Console.WriteLine("Game joined");
    }

    public void handleAvailableMoves(int row, int col)
    {
        int[][] availableMoves = board.getAvailableMovesBoard(row, col, homeTurn);
        List<int[]> availableMovesList = new List<int[]>(availableMoves);

        foreach (int[] move in availableMoves)
        {
            Piece piece = board.getPiece(move[0], move[1]);
            if (piece.getPieceColor() == "W" && homeTurn)
            {
                availableMovesList.Remove(move);
            }
            else if (piece.getPieceColor() == "B" && !homeTurn)
            {
                availableMovesList.Remove(move);
            }
        }

        availableMoves = availableMovesList.ToArray();
        
        var availableMovesInfo = new
        {
            action = "availableMoves",
            availableMoves = availableMoves
        };

        string availableMovesJson = JsonSerializer.Serialize(availableMovesInfo);

        byte[] availableMovesBytes = Encoding.UTF8.GetBytes(availableMovesJson);

        WebSocket playerSocket = homeTurn ? player1.getPlayerSocket() : player2.getPlayerSocket();

        playerSocket.SendAsync(new ArraySegment<byte>(availableMovesBytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public void handleMove(int startRow, int startCol, int endRow, int endCol)
    { 
        bool moved = board.movePiece(startRow, startCol, endRow, endCol);
        if(moved == true && board.kingIsDead())
        {
            Console.WriteLine("Game over here1");
            var gameOverInfo = new
            {
                action = "gameOver",
                winner = homeTurn ? "black" : "white"
            };

            string gameOverJson = JsonSerializer.Serialize(gameOverInfo);

            byte[] gameOverBytes = Encoding.UTF8.GetBytes(gameOverJson);

            player1.getPlayerSocket().SendAsync(new ArraySegment<byte>(gameOverBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            player2.getPlayerSocket().SendAsync(new ArraySegment<byte>(gameOverBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            turnCount++;
            return;
        }

        homeTurn = !homeTurn;
        string[,] boardState = board.getBoardToClient();
        string[][] jaggedBoardState = ConvertToJaggedArray(boardState);

        var boardInfo = new
        {
            action = "movedPiece",
            boardState = jaggedBoardState,
            moved = moved,
            homeTurn = homeTurn
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);

        player1.getPlayerSocket().SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        player2.getPlayerSocket().SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public Guid getGameId()
    {
        return gameId;
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
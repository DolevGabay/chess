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
        string[][] jaggedBoardState = board.getBoardToClient2Perspective();
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
        if (!homeTurn)
        {
            var regularIndices = this.ConvertToRegularPerspective(row, col);

            // Extract regular row and column indices
            row = regularIndices.Item1;
            col = regularIndices.Item2;
        }

        int[][] availableMoves = board.getAvailableMovesBoard(row, col, homeTurn);
        List<int[]> availableMovesList = new List<int[]>(availableMoves);

        // Loop through available moves
        for (int i = 0; i < availableMoves.Length; i++)
        {
            int[] move = availableMoves[i];
            // If it's player 1's turn and the move is from player 2's perspective
            if (!homeTurn)
            {
                // Convert the move indices to player 2's perspective
                var player2Indices = ConvertToPlayer2Perspective(move[0], move[1]);
                // Update the move with player 2's perspective indices
                availableMoves[i] = player2Indices;
            }
        }

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

    // Function to convert indices to player 2's perspective
    public int[] ConvertToPlayer2Perspective(int regularRow, int regularCol)
    {
         // Assuming you have a method to get the number of rows
        int player2Row = 8 - 1 - regularRow;
        int player2Col = 8 - 1 -regularCol;
        return new int[] { player2Row, player2Col };
    }

    public void handleMove(int startRow, int startCol, int endRow, int endCol)
    { 
        if (!homeTurn)
        {
            var regularStartIndices = this.ConvertToRegularPerspective(startRow, startCol);
            var regularEndIndices = this.ConvertToRegularPerspective(endRow, endCol);

            // Extract regular row and column indices
            startRow = regularStartIndices.Item1;
            startCol = regularStartIndices.Item2;
            endRow = regularEndIndices.Item1;
            endCol = regularEndIndices.Item2;
        }

        bool moved = board.movePiece(startRow, startCol, endRow, endCol);

        if(moved == true && board.kingIsDead())
        {
            var gameOverInfo = new
            {
                action = "gameOver",
                winner = homeTurn ? "black" : "white"
            };

            if(homeTurn)
            {
                player1.getUser().scorePoint();
            }
            else
            {
                player2.getUser().scorePoint();
            }

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
        
        string[][] jaggedBoardStateForPlayer2 = board.getBoardToClient2Perspective();    
        
        var boardInfoPlayer1 = new
        {
            action = "movedPiece",
            boardState = jaggedBoardState,
            moved = moved,
            homeTurn = homeTurn
        };

        var boardInfoPlayer2 = new
        {
            action = "movedPiece",
            boardState = jaggedBoardStateForPlayer2,
            moved = moved,
            homeTurn = homeTurn
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfoPlayer1, options);
        var boardInfoBytesPlayer2 = JsonSerializer.SerializeToUtf8Bytes(boardInfoPlayer2, options);

        player1.getPlayerSocket().SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        player2.getPlayerSocket().SendAsync(new ArraySegment<byte>(boardInfoBytesPlayer2), WebSocketMessageType.Text, true, CancellationToken.None);
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

    private (int, int) ConvertToRegularPerspective(int rowFromP2, int colFromP2)
    {
        // Invert row index to convert to regular perspective
        int regularRow = 8 - 1 - rowFromP2;
        // Columns remain the same
        int regularCol = 8 - 1 - colFromP2;

        return (regularRow, regularCol);
    }

    public bool hasTwoPlayers()
    {
        return player1 != null && player2 != null;
    }

    public async void reconnectPlayer1(Player player)
    {
        player1 = player;

        string[,] boardState = board.getBoardToClient();

        Guid playerId = player1.getPlayerId();
        
        // Convert multi-dimensional array to jagged array
        string[][] jaggedBoardState = ConvertToJaggedArray(boardState);
        
        // Create an anonymous object to represent the board info
        var boardInfo = new
        {
            action = "newGame",
            gameId = gameId,
            playerId = playerId,
            boardState = jaggedBoardState,
            home = homeTurn,
            gameStarted = false
        };

        // Serialize the anonymous object to JSON using System.Text.Json
        var options = new JsonSerializerOptions { WriteIndented = true };
        var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);

        // Send the serialized JSON data to the client
        await player1.getPlayerSocket().SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine("reconnected player 1");
    }

    public void removePlayer(WebSocket playerSocket)
    {
        var quitNotify = new
        {
            action = "quit",
            message = "Your opponent has quit the game"
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var message = JsonSerializer.SerializeToUtf8Bytes(quitNotify, options);

        if(player1 != null && player1.getPlayerSocket() == playerSocket)
        {
            player1 = null;
            if(player2 != null && player2.getUser() != null)
            {
                player2.getUser().scorePoint();
            }
            if(player2 != null)
            {
                player2.getPlayerSocket().SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
                player2.getPlayerSocket().CloseAsync(WebSocketCloseStatus.NormalClosure, "Player 1 has quit the game", CancellationToken.None);
            }
        }
        else if(player2 != null && player2.getPlayerSocket() == playerSocket)
        {
            player2 = null;
            if(player1 != null && player1.getUser() != null)
            {
                player1.getUser().scorePoint();
            }

            if(player1 != null)
            {
                player1.getPlayerSocket().SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
                player1.getPlayerSocket().CloseAsync(WebSocketCloseStatus.NormalClosure, "Player 2 has quit the game", CancellationToken.None);
            }
        }
    }

    public Guid getGameId()
    {
        return gameId;
    }

    public Player getPlayer1()
    {
        return player1;
    }

    public Player getPlayer2()
    {
        return player2;
    }
}
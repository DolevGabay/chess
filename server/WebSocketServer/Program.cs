using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();
GameEngine gameEngine = new GameEngine();

app.Map("/start-game", async (HttpContext context) =>
{        
    if (context.WebSockets.IsWebSocketRequest)
    {
        string queryString = context.Request.QueryString.Value;
        string username = null;

        if (!string.IsNullOrEmpty(queryString))
        {
            var queryCollection = System.Web.HttpUtility.ParseQueryString(queryString);
            username = queryCollection["username"];
        }

        Console.WriteLine("Username: " + username);

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket for start-game connection established");

        string Origin = context.Request.Headers["Origin"];
        Console.WriteLine($"WebSocket connection established from host: {Origin} for start-game");
        
        Player player = new Player(socket);
        Game game = new Game(player);
        gameEngine.addGame(game);
        if(username != null)
        {
            gameEngine.addGameToUser(username, game.getGameId());
        }
        
        var handler = new WebSocketHandler();
        await handler.HandleWebSocket(socket, gameEngine, game, CancellationToken.None); // Pass CancellationToken.None
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Map("/join-game/{gameId}", async (HttpContext context) =>
{        
    if (context.WebSockets.IsWebSocketRequest)
    {
        string gameId = context.Request.RouteValues["gameId"] as string;
        if (string.IsNullOrEmpty(gameId))
        {
            context.Response.StatusCode = 400;
            return;
        }

        if(!gameEngine.gameExists(gameId))
        {
            context.Response.StatusCode = 404;
            return;
        }

        Game game = gameEngine.getGameById(gameId);

        if (game.hasTwoPlayers())
        {
            Console.WriteLine("Game already has two players");
            context.Response.StatusCode = 400;
            return;
        }
        
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine($"WebSocket for join-game connection established for game ID: {gameId}");

        string Origin = context.Request.Headers["Origin"];
        Console.WriteLine($"WebSocket connection established from host: {Origin} for join-game");

        Player player = new Player(socket);
        gameEngine.addPlayerToGame(gameId, player);
        
        var handler = new WebSocketHandler();
        await handler.HandleWebSocket(socket, gameEngine, game, CancellationToken.None); 
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Map("/last-game", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        string queryString = context.Request.QueryString.Value;
        string username = null;

        if (!string.IsNullOrEmpty(queryString))
        {
            var queryCollection = System.Web.HttpUtility.ParseQueryString(queryString);
            username = queryCollection["username"];
        }

        Console.WriteLine("Username: " + username);

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket for last-game connection established");
        
        Player player = new Player(socket);
        Game game = gameEngine.getLastGameForUser(username);  
        game.reconnectPlayer1(player);
        
        var handler = new WebSocketHandler();
        await handler.HandleWebSocket(socket, gameEngine, game, CancellationToken.None); 
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Map("/login", async (HttpContext context) =>
{        
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket for start-game connection established");

        string Origin = context.Request.Headers["Origin"];
        Console.WriteLine($"WebSocket connection established from host: {Origin} for start-game");

        var handler = new WebSocketHandler();
        await handler.HandleWebSocket(socket, gameEngine, null, CancellationToken.None); // Pass CancellationToken.None
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();

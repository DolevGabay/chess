using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();
GameEngine gameEngine = new GameEngine();

app.Map("/start-game", async (HttpContext context) =>
{        
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket for start-game connection established");
        
        Player player = new Player(socket);
        //gameEngine.addPlayer(player);
        Game game = new Game(player);
        gameEngine.addGame(game);

        var handler = new WebSocketHandler();
        await handler.HandleWebSocket(socket, game, CancellationToken.None); // Pass CancellationToken.None
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
        
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine($"WebSocket for join-game connection established for game ID: {gameId}");
        Player player = new Player(socket);
        gameEngine.addPlayerToGame(gameId, player);
        Game game = gameEngine.getGameById(gameId);
        
        var handler = new WebSocketHandler();
        await handler.HandleWebSocket(socket, game, CancellationToken.None); 
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});


app.Run();

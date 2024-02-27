using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System;
using System.Text.Json;
using System.Text;

public class GameEngine
{
    private List<Game> games;
    private List<User> users;

    public GameEngine()
    {
        games = new List<Game>();
        users = new List<User>();
    }

    public void addGame(Game game)
    {
        games.Add(game);
    }

    public void addPlayerToGame(string gameId, Player player)
    {
        bool gameExists = false;
        foreach (Game game in games)
        {
            if (game.getGameId().ToString() == gameId)
            {
                game.addPlayer(player);
                gameExists = true;
            }
        }
        if (!gameExists)
        {
            Console.WriteLine("Game does not exist");
        }
    }
    public bool gameExists(string gameId)
    {
        foreach (Game game in games)
        {
            if (game.getGameId().ToString() == gameId)
            {
                return true;
            }
        }
        return false;
    }

    public Game getGameById(string gameId)
    {
        foreach (Game game in games)
        {
            if (game.getGameId().ToString() == gameId)
            {
                return game;
            }
        }
        return null;
    }

    public List<Game> getGames()
    {
        return games;
    }

    public List<User> getUsers()
    {
        return users;
    }

    public void addUser(string username, string password)
    {
        User user = new User(username, password);
        users.Add(user);
    }

    public bool userExists(string userId)
    {
        foreach (User user in this.getUsers())
        {
            if (user.getUsername().ToString() == userId)
            {
                return true;
            }
        }
        return false;
    }

    private bool checkUser(string username, string password)
    {
        foreach (User user in this.getUsers())
        {
            if (user.getUsername().ToString() == username && user.getPassword().ToString() == password)
            {
                return true;
            }
        }
        return false;
    }

    public void addGameToUser(string username, Guid gameId)
    {
        foreach (User user in this.getUsers())
        {
            if (user.getUsername().ToString() == username)
            {
                user.addGame(gameId);
            }
        }
    }

    public Game getLastGameForUser(string username)
    {
        foreach (User user in this.getUsers())
        {
            if (user.getUsername().ToString() == username)
            {
                return this.getGameById(user.getLastGame().ToString());
            }
        }
        return null;
    }

    public async void handleSignup(WebSocket socket, string username, string password)
    {
        if (userExists(username))
        {
            var boardInfo = new
            {
                success = false,
                username = username
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);
            await socket.SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            addUser(username, password);

            var boardInfo = new
            {
                success = true,
                username = username
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);
            await socket.SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        }
    }

    public async void handleSignin(WebSocket socket, string username, string password)
    {
        if (checkUser(username, password))
        {
            var boardInfo = new
            {
                success = true,
                username = username
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);
            await socket.SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            var boardInfo = new
            {
                success = false,
                username = username
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var boardInfoBytes = JsonSerializer.SerializeToUtf8Bytes(boardInfo, options);
            await socket.SendAsync(new ArraySegment<byte>(boardInfoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
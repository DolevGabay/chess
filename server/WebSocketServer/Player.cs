using System;
using System.Net.WebSockets;

public class Player
{
    Guid PlayerId;
    WebSocket socket;
    public bool isHome;
    User user;

    public Player(WebSocket socket, User userToPlayer)
    {
        this.PlayerId = Guid.NewGuid();
        this.socket = socket;
        this.user = userToPlayer;
        Console.WriteLine("Player created with id: " + PlayerId);
    }

    public Guid getPlayerId()
    {
        return PlayerId;
    }

    public WebSocket getPlayerSocket()
    {
        return socket;
    }

    public void setHome(bool home)
    {
        isHome = home;
    }

    public User getUser()
    {
        return user;
    }

    public WebSocket getSocket()
    {
        return socket;
    }
}
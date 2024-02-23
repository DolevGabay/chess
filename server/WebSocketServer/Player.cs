using System;
using System.Net.WebSockets;

public class Player
{
    Guid PlayerId;
    WebSocket socket;
    public bool isHome;

    public Player(WebSocket socket)
    {
        this.PlayerId = Guid.NewGuid();
        this.socket = socket;
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
}
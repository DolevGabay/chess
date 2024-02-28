
public class User
{
    private string username;
    private string password;
    private List<Guid> games;
    private int score;

    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
        games = new List<Guid>();
        score = 0;
    }

    public string getUsername()
    {
        return username;
    }

    public string getPassword()
    {
        return password;
    }

    public void addGame(Guid gameId)
    {
        games.Add(gameId);
        Console.WriteLine("Game added to user: " + username);
    }

    public List<Guid> getGames()
    {
        return games;
    }

    public Guid getLastGame()
    {
        return games[games.Count - 1];
    }

    public int getScore()
    {
        return score;
    }

    public void scorePoint()
    {
        score++;
        Console.WriteLine("User: " + username + " scored a point");
    }
}
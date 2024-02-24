public class GameEngine
{
    private List<Game> games;
    private List<Player> players;

    public GameEngine()
    {
        games = new List<Game>();
    }

    public void addGame(Game game)
    {
        games.Add(game);
    }

    public void addPlayer(Player player)
    {
        players.Add(player);
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
}
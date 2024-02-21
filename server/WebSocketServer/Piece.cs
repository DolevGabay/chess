public class Piece
{
    public string color;
    public string type;
    public int x;
    public int y;
    

    public Piece(string color, string type, int x, int y)
    {
        this.color = color;
        this.type = type;
        this.x = x;
        this.y = y;

    }

    public string getPiece()
    {
        return type + color;
    }
}

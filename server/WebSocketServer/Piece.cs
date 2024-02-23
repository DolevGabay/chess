public class Piece
{
    private string color;
    private string type;
    private int x;
    private int y;
    private bool hasMoved;
    

    public Piece(string color, string type, int x, int y)
    {
        this.color = color;
        this.type = type;
        this.x = x;
        this.y = y;
        this.hasMoved = false;
    }

    public string getPiece()
    {
        return type + color;
    }

    public string getPieceType()
    {
        return type;
    }

    public string getPieceColor()
    {
        return color;
    }

    public void setPieceType(string type)
    {
        this.type = type;
    }

    public void setPieceColor(string color)
    {
        this.color = color;
    }

    public void changeHasMoved()
    {
        hasMoved = true;
    }

    public bool getHasMoved()
    {
        return hasMoved;
    }
}
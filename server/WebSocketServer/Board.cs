using System;

public class Board
{
    private Guid boardId;
    private Piece[,] board;
    private int boardSize = 8;

    public Board()
    {
        boardId = Guid.NewGuid();

        board = new Piece[boardSize, boardSize];
        board[0, 0] = new Piece("B", "R", 0, 0);
        board[0, 1] = new Piece("B", "H", 0, 1);
        board[0, 2] = new Piece("B", "B", 0, 2);
        board[0, 3] = new Piece("B", "Q", 0, 3);
        board[0, 4] = new Piece("B", "K", 0, 4);
        board[0, 5] = new Piece("B", "B", 0, 5);
        board[0, 6] = new Piece("B", "H", 0, 6);
        board[0, 7] = new Piece("B", "R", 0, 7);

        for (int i = 0; i < boardSize; i++)
        {
            board[1, i] = new Piece("B", "P", 1, i);
        }

        for (int i = 2; i < 6; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                board[i, j] = new Piece("E", "E", i, j);
            }
        }

        for (int i = 0; i < boardSize; i++)
        {
            board[6, i] = new Piece("W", "P", 6, i);
        }
        board[7, 0] = new Piece("W", "R", 7, 0);
        board[7, 1] = new Piece("W", "H", 7, 1);
        board[7, 2] = new Piece("W", "B", 7, 2);
        board[7, 3] = new Piece("W", "Q", 7, 3);
        board[7, 4] = new Piece("W", "K", 7, 4);
        board[7, 5] = new Piece("W", "B", 7, 5);
        board[7, 6] = new Piece("W", "H", 7, 6);
        board[7, 7] = new Piece("W", "R", 7, 7);
    }

    public Piece[,] getBoard()
    {
        return board;
    }

    public string[,] getBoardToClient()
    {
        string[,] boardToClient = new string[boardSize, boardSize];
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                boardToClient[i, j] = board[i, j].getPiece();
            }
        }
        return boardToClient;
    }

    public Guid getBoardId()
    {
        return boardId;
    }
}

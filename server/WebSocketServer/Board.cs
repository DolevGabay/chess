using System;

public class Board
{
    private Guid boardId;
    private Piece[,] board;
    private int boardSize = 8;
    private List<Piece> deadPieces = new List<Piece>();

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
        Console.WriteLine("Board created");
    }

    public Piece[,] getBoard()
    {
        return board;
    }

    public Piece getPiece(int row, int col)
    {
        return board[row, col];
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

    public bool movePiece(int startRow, int startCol, int endRow, int endCol)
    {
        Piece startPiece = board[startRow, startCol];
        Piece endPiece = board[endRow, endCol];
        if(startPiece.getPieceColor() == endPiece.getPieceColor())
        {
            Console.WriteLine("Invalid move");
            return false;
        }
        if (endPiece.getPieceType() == "K")
        {
            deadPieces.Add(endPiece);
            Console.WriteLine("Game over");
            return true;
        }

        board[endRow, endCol] = startPiece;
        startPiece.changeHasMoved();
        board[startRow, startCol] = new Piece("E", "E", startRow, startCol);

        if (endPiece.getPieceType() != "E")
        {
            deadPieces.Add(endPiece);
            Console.WriteLine("Piece captured: " + endPiece.getPiece() + " " + endPiece.getPieceColor() + " " + endPiece.getPieceType());
        }
        return true;
    }

    public bool kingIsDead()
    {
        foreach (Piece piece in deadPieces)
        {
            if (piece.getPieceType() == "K" || piece.getPieceType() == "K")
            {
                return true;
            }
        }
        return false;
    }

   public int [][] getAvailableMovesBoard(int row, int col, bool homeTurn)
    {
        Piece currtnePiece = board[row, col];
        string pieceType = currtnePiece.getPieceType();
        int [][] arrOfMoves = getAvailableMoves(pieceType, row, col, homeTurn);
        return arrOfMoves;
    }    

    public int[][] getAvailableMoves(string pieceType, int row, int col, bool homeTurn)
    {
        int[][] arrOfMoves = null;

        if (pieceType == "P")
        {
            Console.WriteLine("Pawn");
            arrOfMoves = getAvailableMovesPawn(row, col, homeTurn);
        }
        else if (pieceType == "R")
        {
            Console.WriteLine("Rook");
            arrOfMoves = getAvailableMovesRook(row, col, homeTurn);
        }
        else if (pieceType == "H")
        {
            Console.WriteLine("Knight");
            arrOfMoves = getAvailableMovesKnight(row, col, homeTurn);
        }
        else if (pieceType == "B")
        {
            Console.WriteLine("Bishop");
            arrOfMoves = getAvailableMovesBishop(row, col, homeTurn);
        }
        else if (pieceType == "Q")
        {
            Console.WriteLine("Queen");
            arrOfMoves = getAvailableMovesQueen(row, col, homeTurn);
        }
        else if (pieceType == "K")
        {
            Console.WriteLine("King");
            arrOfMoves = getAvailableMovesKing(row, col, homeTurn);
        }
        return arrOfMoves;
    }

    public int[][] getAvailableMovesPawn(int row, int col, bool homeTurn)
    {
        List<int[]> availableMovesList = new List<int[]>(); // Use a list to store available moves
        bool pawnMoved = board[row, col].getHasMoved();
        Console.WriteLine("Pawn moved?: " + pawnMoved);
        if (homeTurn)
        {
            if(board[row - 1, col].getPieceType() == "E")
            {
                
                availableMovesList.Add(new int[] { row - 1, col }); // Move one square ahead
                if (!pawnMoved && board[row - 2, col].getPieceType() == "E")
                {
                    availableMovesList.Add(new int[] { row - 2, col }); // Move two squares ahead
                }
                if (board[row - 1, col - 1].getPieceColor() == "B" )
                {
                    availableMovesList.Add(new int[] { row - 1, col - 1 }); // Move one square ahead
                }
                if (board[row - 1, col + 1].getPieceColor() == "B" )
                {
                    availableMovesList.Add(new int[] { row - 1, col + 1 }); // Move one square ahead
                }
            }
            else if (board[row - 1, col - 1].getPieceColor() == "B")
            {
                
                availableMovesList.Add(new int[] { row - 1, col - 1 }); // Move one square ahead
            }
            else if (board[row - 1, col + 1].getPieceColor() == "B")
            {
                
                availableMovesList.Add(new int[] { row - 1, col + 1 }); // Move one square ahead
            }
        }
        else
        {
            if (board[row + 1, col].getPieceType() == "E")
            {
                availableMovesList.Add(new int[] { row + 1, col }); // Move one square ahead
                if (!pawnMoved && board[row + 2, col].getPieceType() == "E")
                {
                    availableMovesList.Add(new int[] { row + 2, col }); // Move two squares ahead
                }
                if (board[row + 1, col - 1].getPieceColor() == "W")
                {
                    availableMovesList.Add(new int[] { row + 1, col - 1 }); // Move one square ahead
                }
                if (board[row + 1, col + 1].getPieceColor() == "W")
                {
                    availableMovesList.Add(new int[] { row + 1, col + 1 }); // Move one square ahead
                }
            }
            else if (board[row + 1, col - 1].getPieceColor() == "W")
            {
                availableMovesList.Add(new int[] { row + 1, col - 1 }); // Move one square ahead
            }
            else if (board[row + 1, col + 1].getPieceColor() == "W")
            {
                availableMovesList.Add(new int[] { row + 1, col + 1 }); // Move one square ahead
            }
        }

        int[][] availableMoves = availableMovesList.ToArray();

        return availableMoves;
    }

    public int[][] getAvailableMovesRook(int row, int col, bool homeTurn)
    {
        List<int[]> availableMovesList = new List<int[]>(); // Use a list to store available moves
        int r = row;
        while (true)
        {
            // Move up
            r--;
            if (r < 0) break;
            if (board[r, col].getPieceType() == "E")
            {
                availableMovesList.Add(new int[] { r, col });
            }
            else if (board[r, col].getPieceColor() != board[row, col].getPieceColor())
            {
                availableMovesList.Add(new int[] { r, col });
                break;
            }
            else
            {
                break;
            }
        }

        r = row;
        while (true)
        {
            // Move down
            r++;
            if (r >= boardSize) break;
            if (board[r, col].getPieceType() == "E")
            {
                availableMovesList.Add(new int[] { r, col });
            }
            else if (board[r, col].getPieceColor() != board[row, col].getPieceColor())
            {
                availableMovesList.Add(new int[] { r, col });
                break;
            }
            else
            {
                break;
            }
        }

        int c = col;
        while (true)
        {
            // Move left
            c--;
            if (c < 0) break;
            if (board[row, c].getPieceType() == "E")
            {
                availableMovesList.Add(new int[] { row, c });
            }
            else if (board[row, c].getPieceColor() != board[row, col].getPieceColor())
            {
                availableMovesList.Add(new int[] { row, c });
                break;
            }
            else
            {
                break;
            }
        }

        c = col;
        while (true)
        {
            // Move right
            c++;
            if (c >= boardSize) break;
            if (board[row, c].getPieceType() == "E")
            {
                availableMovesList.Add(new int[] { row, c });
            }
            else if (board[row, c].getPieceColor() != board[row, col].getPieceColor())
            {
                availableMovesList.Add(new int[] { row, c });
                break;
            }
            else
            {
                break;
            }
        }

        // Convert the list of available moves to a jagged array
        int[][] availableMoves = availableMovesList.ToArray();

        return availableMoves;
    }

    public int[][] getAvailableMovesKnight(int row, int col, bool homeTurn)
    {
        List<int[]> availableMovesList = new List<int[]>(); // Use a list to store available moves

        // Define all possible knight move offsets
        int[][] knightMoveOffsets = new int[][] 
        {
            new int[] { -2, -1 }, // Up 2, Left 1
            new int[] { -2, 1 },  // Up 2, Right 1
            new int[] { -1, -2 }, // Up 1, Left 2
            new int[] { -1, 2 },  // Up 1, Right 2
            new int[] { 1, -2 },  // Down 1, Left 2
            new int[] { 1, 2 },   // Down 1, Right 2
            new int[] { 2, -1 },  // Down 2, Left 1
            new int[] { 2, 1 }    // Down 2, Right 1
        };

        // Iterate through all possible knight move offsets
        foreach (int[] offset in knightMoveOffsets)
        {
            int newRow = row + offset[0];
            int newCol = col + offset[1];

            // Check if the new position is within the board boundaries
            if (newRow >= 0 && newRow < boardSize && newCol >= 0 && newCol < boardSize)
            {
                availableMovesList.Add(new int[] { newRow, newCol });
            }
        }

        // Convert the list of available moves to a jagged array
        int[][] availableMoves = availableMovesList.ToArray();

        return availableMoves;
    }


    public int[][] getAvailableMovesBishop(int row, int col, bool homeTurn)
    {
        List<int[]> availableMovesList = new List<int[]>(); // Use a list to store available moves

        // Define all possible diagonal move directions (up-left, up-right, down-left, down-right)
        int[][] diagonalDirections = new int[][] 
        {
            new int[] { -1, -1 }, // Up-left
            new int[] { -1, 1 },  // Up-right
            new int[] { 1, -1 },  // Down-left
            new int[] { 1, 1 }    // Down-right
        };

        // Iterate through all possible diagonal move directions
        foreach (int[] direction in diagonalDirections)
        {
            int dRow = direction[0];
            int dCol = direction[1];

            int newRow = row + dRow;
            int newCol = col + dCol;

            // Continue moving along the diagonal until reaching the edge of the board
            while (newRow >= 0 && newRow < boardSize && newCol >= 0 && newCol < boardSize)
            {
                if (board[newRow, newCol].getPieceType() == "E")
                {
                    availableMovesList.Add(new int[] { newRow, newCol });    
                }
                else if (board[newRow, newCol].getPieceColor() != board[row, col].getPieceColor())
                {
                    availableMovesList.Add(new int[] { newRow, newCol });
                    break;
                }
                else if(board[newRow, newCol].getPieceColor() == board[row, col].getPieceColor())
                {
                    break;
                }
                else
                {
                    break;
                }
                // Move to the next position along the diagonal
                newRow += dRow;
                newCol += dCol;
            }
        }

        // Convert the list of available moves to a jagged array
        int[][] availableMoves = availableMovesList.ToArray();

        return availableMoves;
    }


    public int[][] getAvailableMovesQueen(int row, int col, bool homeTurn)
    {
        List<int[]> availableMovesList = new List<int[]>(); // Use a list to store available moves

        // Define all possible diagonal move directions (up-left, up-right, down-left, down-right)
        int[][] diagonalDirections = new int[][] 
        {
            new int[] { -1, -1 }, // Up-left
            new int[] { -1, 1 },  // Up-right
            new int[] { 1, -1 },  // Down-left
            new int[] { 1, 1 }    // Down-right
        };

        // Define all possible straight move directions (up, down, left, right)
        int[][] straightDirections = new int[][] 
        {
            new int[] { -1, 0 }, // Up
            new int[] { 1, 0 },  // Down
            new int[] { 0, -1 }, // Left
            new int[] { 0, 1 }   // Right
        };

        // Iterate through all possible diagonal move directions
        foreach (int[] direction in diagonalDirections)
        {
            int dRow = direction[0];
            int dCol = direction[1];

            int newRow = row + dRow;
            int newCol = col + dCol;

            // Continue moving along the diagonal until reaching the edge of the board
            while (newRow >= 0 && newRow < boardSize && newCol >= 0 && newCol < boardSize)
            {
                if (board[newRow, newCol].getPieceType() == "E")
                {
                    availableMovesList.Add(new int[] { newRow, newCol });    
                }
                else if (board[newRow, newCol].getPieceColor() != board[row, col].getPieceColor())
                {
                    availableMovesList.Add(new int[] { newRow, newCol });
                    break;
                }
                else if(board[newRow, newCol].getPieceColor() == board[row, col].getPieceColor())
                {
                    break;
                }
                else
                {
                    break;
                }

                // Move to the next position along the diagonal
                newRow += dRow;
                newCol += dCol;
            }
        }

        // Iterate through all possible straight move directions
        foreach (int[] direction in straightDirections)
        {
            int dRow = direction[0];
            int dCol = direction[1];

            int newRow = row + dRow;
            int newCol = col + dCol;

            // Continue moving along the straight direction until reaching the edge of the board
            while (newRow >= 0 && newRow < boardSize && newCol >= 0 && newCol < boardSize)
            {
                if (board[newRow, newCol].getPieceType() == "E")
                {
                    availableMovesList.Add(new int[] { newRow, newCol });    
                }
                else if (board[newRow, newCol].getPieceColor() != board[row, col].getPieceColor())
                {
                    availableMovesList.Add(new int[] { newRow, newCol });
                    break;
                }
                else if(board[newRow, newCol].getPieceColor() == board[row, col].getPieceColor())
                {
                    break;
                }
                else
                {
                    break;
                }

                // Move to the next position along the straight direction
                newRow += dRow;
                newCol += dCol;
            }
        }

        // Convert the list of available moves to a jagged array
        int[][] availableMoves = availableMovesList.ToArray();

        return availableMoves;
    }


    public int[][] getAvailableMovesKing(int row, int col, bool homeTurn)
    {
        List<int[]> availableMovesList = new List<int[]>(); // Use a list to store available moves

        // Define all possible move directions (including diagonals)
        int[][] moveDirections = new int[][] 
        {
            new int[] { -1, -1 }, // Up-left
            new int[] { -1, 0 },  // Up
            new int[] { -1, 1 },  // Up-right
            new int[] { 0, -1 },  // Left
            new int[] { 0, 1 },   // Right
            new int[] { 1, -1 },  // Down-left
            new int[] { 1, 0 },   // Down
            new int[] { 1, 1 }    // Down-right
        };

        // Iterate through all possible move directions
        foreach (int[] direction in moveDirections)
        {
            int dRow = direction[0];
            int dCol = direction[1];

            int newRow = row + dRow;
            int newCol = col + dCol;

            // Check if the new position is within the board boundaries
            if (newRow >= 0 && newRow < boardSize && newCol >= 0 && newCol < boardSize)
            {
                availableMovesList.Add(new int[] { newRow, newCol });
            }
        }

        // Convert the list of available moves to a jagged array
        int[][] availableMoves = availableMovesList.ToArray();

        return availableMoves;
    }
    
}

import React, { useState, useEffect } from 'react';
import './Chessboard.css'; // Import CSS file for chessboard styling
import { BoardUtils } from './BoardUtils'; // Adjust the path based on the actual location of BoardUtils.js

const Chessboard = () => {
    const [board, setBoard] = useState();
    const [playerID, setPlayerID] = useState(0);
    
    useEffect(() => {
        const newBoard = Array(8).fill(null).map(() => Array(8).fill('E'));
        newBoard[0] = [['R','W'], ['H','W'], ['B','W'], ['Q','W'], ['K','W'], ['B','W'], ['H','W'], ['R','W']];
        newBoard[1] = Array(8).fill(['P','W']);
        newBoard[6] = Array(8).fill(['P','B']);
        newBoard[7] = [['R','B'], ['H','B'], ['B','B'], ['Q','B'], ['K','B'], ['B','B'], ['H','B'], ['R','B']];
        setBoard(newBoard);
    }, []);
    

    const handleClick = (row, col, piece) => {
        console.log(`Clicked on row ${row}, col ${col}, piece ${piece}`);
        BoardUtils.drawPossibleMoves(row, col, piece);
    };

    return (
        <div className="chessboard">
            {board && board.map((row, i) => (
                <div key={i} className="row">
                    {row.map((piece, j) => (
                        <div key={j} className={((i + j) % 2 === 0) ? 'square white' : 'square black'} onClick={() => handleClick(i, j, piece[0])}>
                            <div className="card">
                                {BoardUtils.setCellFigure(piece[0], piece[1])}
                            </div>
                        </div>
                    ))}
                </div>
            ))}
        </div>
    );
}

export default Chessboard;

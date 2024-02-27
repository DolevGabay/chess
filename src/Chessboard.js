import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './Chessboard.css';
import { BoardUtils } from './BoardUtils';

const Chessboard = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const searchParams = new URLSearchParams(location.search);
    const mode = searchParams.get('mode');
    const gameToJoin = searchParams.get('gameId');
    const username = searchParams.get('username');

    const [board, setBoard] = useState([]);
    const [playerID, setPlayerID] = useState(0);
    const [gameID, setGameID] = useState(0);
    const [home, setHome] = useState(false);
    const [socket, setSocket] = useState(null);
    const [availableMoves, setAvailableMoves] = useState([]);
    const [clickedOnPiece, setClickedOnPiece] = useState(false);
    const [selectedPiece, setSelectedPiece] = useState(null);
    const [homeTurn, setHomeTurn] = useState(true);
    const [gameStarted, setGameStarted] = useState(false);

    useEffect(() => {
        
        const connectToWebSocket = () => {
            let socketURL = '';
            if (mode === 'start') {
                socketURL = `ws://localhost:5226/start-game?username=${username}`;
            } else if (mode === 'join') {
                socketURL = 'ws://localhost:5226/join-game';
            } else if(mode === 'last-game') {
                socketURL = `ws://localhost:5226/last-game?username=${username}`;
            } else {
                console.error('Invalid mode:', mode);
                return;
            }

            if (mode === 'join' && gameToJoin) {
                const gameId = gameToJoin;
                socketURL += `/${gameId}`;
            }

            const ws = new WebSocket(socketURL);

            ws.onopen = () => {
                console.log('Connected to WebSocket server');
            };

            ws.onmessage = (event) => {
                const data = JSON.parse(event.data);
                if (data.action === 'newGame') {
                    console.log('Game started');
                    setBoard(data.boardState);
                    setGameID(data.gameId);
                    setHome(data.home);
                    setGameStarted(data.gameStarted);
                    setPlayerID(data.playerId);
                } else if (data.action === 'availableMoves') {
                    console.log('Available moves:', data.availableMoves);
                    setAvailableMoves(data.availableMoves);
                } else if (data.action === 'movedPiece') {
                    console.log('Move:', data);
                    if (data.moved == false){
                        alert('Invalid Move');
                    }
                    else{
                    setHomeTurn(data.homeTurn);
                    console.log('Home turn:', homeTurn);
                    setAvailableMoves([]);
                    setBoard(data.boardState);
                    }
                } else if (data.action === 'gameOver') {
                    console.log('Game over:', data);
                    alert('Game over');
                } else if (data.action === 'gameStarted'){
                    setGameStarted(true);
                }
            };

            ws.onerror = (error) => {
                //check the status number of error
                if (error.target.readyState === 3) {
                    console.log('Game not found');
                    alert('Game not found');
                    navigate('/');
                }
                console.error('WebSocket error:', error);
            };

            ws.onclose = () => {
                console.log('WebSocket connection closed');
            };

            setSocket(ws);
        };

        connectToWebSocket();

        return () => {
            if (socket) {
                socket.close();
            }
        };
    }, [mode, gameToJoin]);

    const handleClick = (row, col, pieceColor) => {
        console.log(`Clicked on row ${row}, col ${col}, piece ${pieceColor}`);
        const clickedCell = [row, col];
        if ((home && !homeTurn) || (!home && homeTurn)) {
            setClickedOnPiece(false);
            setAvailableMoves([]);
            setSelectedPiece(null);
            console.log('Not your turn');
            return;
        }
        if ((board[row][col][0] === 'E' || (home && pieceColor === 'B') || (!home && pieceColor === 'W')) && !availableMoves.some(move => move[0] === row && move[1] === col)) {
            setClickedOnPiece(false);
            setAvailableMoves([]);
            setSelectedPiece(null);
            console.log('Not your piece or empty cell');
            return;
        }

        if (board[row][col][0] === 'E' && !availableMoves.some(move => move[0] === row && move[1] === col)) {
            // If the clicked cell is empty and not in available moves, set clickedOnPiece to false
            setClickedOnPiece(false);
            setAvailableMoves([]);
            setSelectedPiece(null);
            console.log('Clicked on empty cell');
        } else if (!clickedOnPiece) {
            
            setClickedOnPiece(true);
            setSelectedPiece(clickedCell);
            BoardUtils.drawPossibleMoves(row, col, pieceColor, socket);
            console.log('Clicked on piece');
        } else if (clickedOnPiece && !availableMoves.some(move => move[0] === row && move[1] === col)) {
            
            BoardUtils.drawPossibleMoves(row, col, pieceColor, socket);
            setSelectedPiece(clickedCell);
            console.log('Clicked on other piece');
        } else {
            // Handle move logic here
            console.log('Handle move logic');
            const from = selectedPiece;
            const to = clickedCell;
            BoardUtils.movePiece(from, to, socket);
        }
    };

    const isCellAvailable = (row, col) => {
        return availableMoves.some(([r, c]) => r === row && c === col);
    };

    return (
        <div className="chessboard-body">
        <div className="chessboard">
            <h1 style={{ color: 'Black', fontSize: '20px' }}>Player: {username != "" ? username : "Guest"}</h1>
            <h1 style={{ color: 'Black', fontSize: '20px' }}>Game ID: {gameID}</h1>
            {home ? <h2>Your Color: White</h2> : <h2>Your Color: Black</h2>}
            {board.map((row, i) => (
                <div key={i} className="row">
                    {row.map((piece, j) => (
                        <div
                            key={j}
                            className={`square ${((i + j) % 2 === 0) ? 'white' : 'black'} ${isCellAvailable(i, j) ? 'available' : ''}`}
                            onClick={() => handleClick(i, j, piece[1])}
                        >
                            <div className="card">
                                {BoardUtils.setCellFigure(piece[0], piece[1])}
                            </div>
                        </div>
                    ))}
                </div>
            ))}
            <div id="notification-modal" className={gameStarted ? "notification-modal hidden" : "notification-modal"}>
            <h2>Waiting for the other player</h2>
            <h3>Game ID: {gameID}</h3>
            </div>
            {(home && homeTurn) ? <h2>Your Turn</h2> : (home && !homeTurn) ? <h2>Opponent's Turn</h2> : (!home && !homeTurn) ? <h2>Your Turn</h2> : <h2>Opponent's Turn</h2>}
        </div>
        </div>
    );
};

export default Chessboard;

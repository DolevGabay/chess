import { useNavigate } from 'react-router-dom';

export class BoardUtils {
    static setCellFigure(letter, color) {
        if (letter === 'K' && color === 'W') {
            return '♔';
        }
        if (letter === 'Q' && color === 'W') {
            return '♕';
        }
        if (letter === 'R' && color === 'W') {
            return '♖';
        }
        if (letter === 'B' && color === 'W') {
            return '♗';
        }
        if (letter === 'H' && color === 'W') {
            return '♘';
        }
        if (letter === 'P' && color === 'W') {
            return '♙';
        }
        if (letter === 'K' && color === 'B') {
            return '♚';
        }
        if (letter === 'Q' && color === 'B') {
            return '♛';
        }
        if (letter === 'R' && color === 'B') {
            return '♜';
        }
        if (letter === 'B' && color === 'B') {
            return '♝';
        }
        if (letter === 'H' && color === 'B') {
            return '♞';
        }
        if (letter === 'P' && color === 'B') {
            return '♟';
        }
        return '';
    }

    static drawPossibleMoves(row, col, piece, socket) {
        console.log(`Draw possible moves for piece ${piece} at row ${row}, col ${col}`);
        if (socket) {
            // Socket is defined, send data
            socket.send(JSON.stringify({ action: 'availableMoves', row: row, col: col }));
        } else {
            console.error('Socket is undefined');
        }
    }

    static movePiece(from, to, socket) {
        console.log(`Move piece from ${from} to ${to}`);
        if (socket) {
            // Socket is defined, send data
            socket.send(JSON.stringify({ action: 'move', fromRow: from[0], fromCol: from[1], toRow: to[0], toCol: to[1] }));
        } else {
            console.error('Socket is undefined');
        }
    }

    static async login(username, password, action) {
        return new Promise((resolve, reject) => {
            const ws = new WebSocket('ws://localhost:5226/login');
    
            ws.onopen = () => {
                console.log('WebSocket connection opened');
                const data = {
                    username: username,
                    password: password,
                    action: action
                };
                ws.send(JSON.stringify(data));
            }
    
            ws.onerror = (error) => {
                console.error('WebSocket error:', error);
                ws.close(); // Close the connection on error
                reject(error);
            }
    
            ws.onclose = () => {
                console.log('WebSocket connection closed');
            }
    
            ws.onmessage = (message) => {
                console.log('Received message:', message.data);
                const data = JSON.parse(message.data);
                if(data.success === false) {
                    ws.close(); // Close the connection if username exists
                    resolve("Invalid username or password")
                }
                else {
                    const username = data.username;
                    console.log('Username:', username);
                    resolve(username);
                    ws.close(); // Close the connection on success
                }
            }
        });
    }
    
    
}

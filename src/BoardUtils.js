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

    static drawPossibleMoves(row, col, piece) {
        console.log(`Draw possible moves for piece ${piece} at row ${row}, col ${col}`);
    }
}

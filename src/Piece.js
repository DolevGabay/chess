class Piece {
    constructor(color, type) {
      this.color = color;
    }
  
    availableMoves() {
        // Return a list of available moves
    }
  }
  
  class King extends Piece {
    constructor(color) {
      super(color);
      this.type = "King";
    }
  
  }
  
  class Queen extends Piece {
    constructor(color) {
      super(color);
      this.type = "Queen";
    }
  
  }
  
  
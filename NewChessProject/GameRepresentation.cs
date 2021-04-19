using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameRepresentation
    {
        List<PieceRepresentation> pieces;
        List<BoardIndicator> moves;
        List<BoardIndicator> pieceHilights;

        public GameRepresentation(List<PieceRepresentation> pieces = null, List<BoardIndicator> moves = null, List<BoardIndicator> pieceHilights = null)
        {
            this.pieces = pieces;
            if(pieces == null)
            {
                this.pieces = new List<PieceRepresentation>();
            }

            this.moves = moves;
            if (moves == null)
            {
                this.moves = new List<BoardIndicator>();
            }

            this.pieceHilights = pieceHilights;
            if (pieceHilights == null)
            {
                this.pieceHilights = new List<BoardIndicator>();
            }
        }

        public List<PieceRepresentation> Pieces
        {
            get
            {
                return pieces;
            }
            set
            {
                pieces = value;
            }
        }
        public List<BoardIndicator> Moves
        {
            get
            {
                return moves;
            }
            set
            {
                moves = value;
            }
        }
        public List<BoardIndicator> PieceHilights
        {
            get
            {
                return pieceHilights;
            }
            set
            {
                pieceHilights = value;
            }
        }
    }
}

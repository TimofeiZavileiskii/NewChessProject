using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    abstract class Piece
    {
        PieceType type;
        PlayerColour colour;
        protected bool hasMoved;
        protected List<Vector> availableMoves;
        protected List<Vector> directions;
        public Piece(PlayerColour colour, bool hasMoved = false)
        {
            this.colour = colour;
            this.hasMoved = hasMoved;
            directions = new List<Vector>();
            availableMoves = new List<Vector>();
        }

        public List<Vector> AvailableMoves
        {
            get
            {
                return new List<Vector>(availableMoves);
            }
        }

        public PlayerColour Colour
        {
            get
            {
                return colour;
            }
        }

        public PieceType Type
        {
            get
            {
                return type;
            }
            protected set
            {
                type = value;
            }
        }

        abstract public void GenerateMoves(Board board, Vector position);

        public void FilterPositionsByCheck(Board board, Vector currentLocation)
        {
            List<Vector> filteredMoves = new List<Vector>();
            foreach(Vector move in availableMoves)
            {
                Board testingBoard = board.Copy();
                testingBoard.MovePiece(currentLocation, move);
                testingBoard.GenerateMoves(Board.ReverseColour(colour));

                if (!testingBoard.IsChecked(colour))
                    filteredMoves.Add(move);
            }

            availableMoves = filteredMoves;
        }

        public bool HasMoved
        {
            set
            {
                hasMoved = value;
            }
            get
            {
                return hasMoved;
            }
        }

        protected bool CheckInRange(Vector vec)
        {
            return vec.X > -1 && vec.X < Board.boardWidth && vec.Y > -1 && vec.Y < Board.boardHeight;
        }

        public bool MoveExists(Vector in_vec)
        {
            bool output = false;
            foreach(Vector vec in availableMoves)
            {
                if(vec == in_vec)
                {
                    output = true;
                    break;
                }
            }
            return output;
        }

    }
}

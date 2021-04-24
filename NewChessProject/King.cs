using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class King : OneStepPiece
    {
        const int CastleDistance = 2;

        public King(PlayerColour color, bool hasMoved = false) : base(color, hasMoved)
        {
            Type = PieceType.King;
            directions.Add(new Vector(0, 1));
            directions.Add(new Vector(1, 0));
            directions.Add(new Vector(-1, 0));
            directions.Add(new Vector(0, -1));
            directions.Add(new Vector(-1, 1));
            directions.Add(new Vector(-1, -1));
            directions.Add(new Vector(1, 1));
            directions.Add(new Vector(1, -1));
        }

        override public void GenerateMoves(Board board, Vector position)
        {
            base.GenerateMoves(board, position);

            //The king checks is castling possible
            //The king must not move and not be in check
            if (!HasMoved && !board.IsChecked(Colour))
            {
                
                Vector leftPiecePos = new Vector(0, position.Y);
                Piece leftPiece = board[leftPiecePos];
                Vector rightPiecePos = new Vector(Board.boardWidth - 1, position.Y);
                Piece rightPiece = board[rightPiecePos];

                //King checks that there are no pieces between it and the rook and it will not pass through positions under threat
                if (leftPiece != null)
                {
                    bool free = true;

                    if (leftPiece.HasMoved)
                        free = false;
                    
                    //Check that there are no pieces between the king and the rook
                    for (int i = position.X - 1; i > leftPiecePos.X && free; i--)
                    {
                        Vector checkedPosition = new Vector(i, position.Y);
                        if (board[checkedPosition] != null)
                            free = false;
                    }

                    //Check that there are no threats between the king and its final position
                    for (int i = position.X - 1; i >= position.X - CastleDistance && free; i--)
                    {
                        Vector checkedPosition = new Vector(i, position.Y);
                        if (board.IsThereThreat(checkedPosition, Colour))
                            free = false;
                    }
                    
                    if (free)
                        availableMoves.Add(new Vector(position.X - CastleDistance, position.Y, SpecialMove.CastleLeft));
                }

                //Same procedure works for the right rook
                if (rightPiece != null)
                {
                    bool free = true;
                    if (rightPiece.HasMoved)
                        free = false;
                    
                    for (int i = position.X + 1; i < rightPiecePos.X && free; i++)
                    {
                        Vector checkedPosition = new Vector(i, position.Y);
                        if (board[checkedPosition] != null)
                            free = false;
                    }

                    for (int i = position.X + 1; i <= position.X + CastleDistance && free; i++)
                    {
                        Vector checkedPosition = new Vector(i, position.Y);
                        if (board.IsThereThreat(checkedPosition, Colour))
                            free = false;
                    }
                    
                    if (free)
                        availableMoves.Add(new Vector(position.X + CastleDistance, position.Y, SpecialMove.CastleRight));
                }
            }
        }
    }
}

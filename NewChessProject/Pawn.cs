using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class Pawn : Piece
    {
        List<Vector> attackDirections;
        List<Vector> enPassanteAttack;
        bool vulnurableToEnPassante;
        int direction;

        public Pawn(PlayerColour color, bool hasMoved = false) : base(color, hasMoved)
        {
            GenerateDirection();
            directions.Add(new Vector(0, direction));

            attackDirections = new List<Vector>();
            enPassanteAttack = new List<Vector>();
            Type = PieceType.Pawn;
            
            vulnurableToEnPassante = false;

            attackDirections.Add(new Vector(1, direction));
            attackDirections.Add(new Vector(-1, direction));

            enPassanteAttack.Add(new Vector(-1, 0));
            enPassanteAttack.Add(new Vector(1, 0));
        }

        public bool VulnurableToEnPassante
        {
            get
            {
                return vulnurableToEnPassante;
            }
            set
            {
                vulnurableToEnPassante = value;
            }
        }

        public int Direction
        {
            get
            {
                return direction;
            }
        }

        private void GenerateDirection()
        {
            if(Colour == PlayerColour.White)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }

        override public void GenerateMoves(Board board, Vector position)
        {
            //Checks and adds moves to move foward
            availableMoves.Clear();
            Vector checkedPosition = position;
            checkedPosition += directions[0];
            bool freeInFront = false;

            if (CheckInRange(checkedPosition))
                if (board[checkedPosition] == null)
                {
                    availableMoves.Add(checkedPosition);
                    freeInFront = true;
                }

            checkedPosition += directions[0];
            if (CheckInRange(checkedPosition) && !hasMoved && freeInFront)
                if (board[checkedPosition] == null)
                    availableMoves.Add(new Vector(checkedPosition.X, checkedPosition.Y, SpecialMove.DoubleFoward));



            //Checks and adds moves to take at diagonals
            foreach (Vector vec in attackDirections)
            {
                checkedPosition = position + vec;
                if (CheckInRange(checkedPosition))
                    if (board[checkedPosition] != null)
                        if (board[checkedPosition].Colour != Colour)
                            availableMoves.Add(checkedPosition);
            }


            //Checks and adds moves related to en passante
            foreach (Vector vec in enPassanteAttack)
            {
                checkedPosition = position + vec;
                if (CheckInRange(checkedPosition))
                    if (board[checkedPosition] != null)
                        if (board[checkedPosition].Type == PieceType.Pawn && board[checkedPosition].Colour != Colour)
                            if(((Pawn)board[checkedPosition]).VulnurableToEnPassante)
                                availableMoves.Add(new Vector(checkedPosition.X, checkedPosition.Y + direction, SpecialMove.EnPassante));
            }

        }
    }
}

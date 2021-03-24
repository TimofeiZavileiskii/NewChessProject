using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class FENPosition
    {
        string position;
        string currentPlayer;
        string castling;
        string enPassante;
        string halfMoveTimer;
        string totalMoves;

        public string Position
        {
            set
            {
                position = value;
            }
            get
            {
                return position;
            }
        }

        public string CurrentPlayer
        {
            set
            {
                currentPlayer = value;
            }
            get
            {
                return currentPlayer;
            }
        }
        public string Castling
        {
            set
            {
                castling = value;
            }
            get
            {
                return castling;
            }
        }
        public string EnPassante
        {
            set
            {
                enPassante = value;
            }
            get
            {
                return enPassante;
            }
        }
        public string HalfMoveTimer
        {
            set
            {
                halfMoveTimer = value;
            }
            get
            {
                return halfMoveTimer;
            }
        }
        public string TotalMoves
        {
            set
            {
                totalMoves = value;
            }
            get
            {
                return totalMoves;
            }
        }

        public string FENString
        {
            get 
            {
                return position + " " + currentPlayer + " " + castling + " " + enPassante + " " + halfMoveTimer + " " + totalMoves;
            }

        }


        public FENPosition()
        {

        }


    }
}

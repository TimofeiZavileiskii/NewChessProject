using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class MadeMoveEventArgs : EventArgs
    {
        public MadeMoveEventArgs(MoveResult result) : base()
        {
            Result = result;
        }

        public MoveResult Result { get; set; }
    }

    class GameEndedEventArgs : EventArgs
    {
        public PlayerColour Winner
        {
            get;
            set;
        }

        public MoveResult Reason
        {
            get;
            set;
        }

        public GameEndedEventArgs(MoveResult reason, PlayerColour winner)
        {
            Reason = reason;
            Winner = winner;
        }
    }


    class BoardClickedEventArgs : EventArgs
    {
        public BoardClickedEventArgs(Vector vector)
        {
            Position = vector;
        }

        public Vector Position
        {
            get; set;
        }
    }

    class PieceSelectedEventArgs : EventArgs
    {
        public PieceSelectedEventArgs(PieceType type)
        {
            SelectedPieceType = type;
        }

        public PieceType SelectedPieceType { get; set; }
    }
}

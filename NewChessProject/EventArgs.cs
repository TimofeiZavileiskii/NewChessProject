﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameStartEventArgs : EventArgs
    {
        public bool OneHumanPlayer
        {
            get;
            set;
        }

        public PlayerColour StartingPlayer
        {
            get;
            set;
        }

        public GameStartEventArgs(bool oneHumanPlayer, PlayerColour colour) : base()
        {
            OneHumanPlayer = oneHumanPlayer;
            StartingPlayer = colour;
        }
    }
    class MadeMoveEventArgs : EventArgs
    {
        public MadeMoveEventArgs(MoveResult result, PlayerColour colour) : base()
        {
            Result = result;
            PlayerToMove = colour;
        }

        public MoveResult Result { get; set; }
        public PlayerColour PlayerToMove { get; set; }
    }

    class GameEndedEventArgs : EventArgs
    {
        public PlayerColour? Winner
        {
            get;
            set;
        }

        public MoveResult Reason
        {
            get;
            set;
        }

        public GameEndedEventArgs(MoveResult reason, PlayerColour? winner)
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

    class RequestMadeEventArgs : EventArgs
    {
        public Request Request { get; set; }

        public RequestMadeEventArgs(Request request)
        {
            Request = request;
        }
    }
}

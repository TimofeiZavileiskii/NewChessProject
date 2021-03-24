using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class Board
    {
        Piece[,] field;
        Vector[] kingPositions;
        Vector enPassantePiece;


        public const int boardHeight = 8;
        public const int boardWidth = 8;

        public Board()
        {
            field = new Piece[boardHeight, boardWidth];
            enPassantePiece = new Vector(-1, -1);
            int numberOfColours = Enum.GetValues(typeof(PlayerColour)).Length;
            kingPositions = new Vector[numberOfColours];

        }


        public PlayerColour ReverseColour(PlayerColour colour)
        {
            PlayerColour output = PlayerColour.Black;
            if(colour == PlayerColour.Black)
                output = PlayerColour.White;
            return output;
        }

        public void TransformPawn(PlayerColour colour, PieceType piece)
        {
            this[LocatePawnNeedingTransformation(colour)] = MakePiece(piece, colour, true);
        }

        public bool IsPawnTransformationNeeded(PlayerColour colour)
        {
            bool output = false;
            if (LocatePawnNeedingTransformation(colour) != new Vector(-1, -1))
                output = true;

            return output;
        }

        private Vector LocatePawnNeedingTransformation(PlayerColour colour)
        {
            int y = 0;
            if (colour == PlayerColour.White)
                y = boardHeight - 1;

            for (int i = 0; i < boardWidth; i++)
                if (field[i, y] != null)
                    if (field[i, y].Type == PieceType.Pawn)
                        return new Vector(i, y);
            return new Vector(-1, -1);
        }

        public void SetDefaultBoardPosition()
        {
            AddPiece(new Bishop(PlayerColour.White), new Vector(2, 0));
            AddPiece(new Bishop(PlayerColour.White), new Vector(5, 0));

            AddPiece(new Bishop(PlayerColour.Black), new Vector(2, boardHeight - 1));
            AddPiece(new Bishop(PlayerColour.Black), new Vector(5, boardHeight - 1));

            AddPiece(new Knight(PlayerColour.White), new Vector(1, 0));
            AddPiece(new Knight(PlayerColour.White), new Vector(6, 0));

            AddPiece(new Knight(PlayerColour.Black), new Vector(1, boardHeight - 1));
            AddPiece(new Knight(PlayerColour.Black), new Vector(6, boardHeight - 1));

            AddPiece(new Rook(PlayerColour.White), new Vector(0, 0));
            AddPiece(new Rook(PlayerColour.White), new Vector(7, 0));

            AddPiece(new Rook(PlayerColour.Black), new Vector(0, boardHeight - 1));
            AddPiece(new Rook(PlayerColour.Black), new Vector(7, boardHeight - 1));

            AddPiece(new Queen(PlayerColour.White), new Vector(3, 0));
            AddPiece(new Queen(PlayerColour.Black), new Vector(3, boardHeight - 1));

            AddPiece(new King(PlayerColour.White), new Vector(4, 0));
            AddPiece(new King(PlayerColour.Black), new Vector(4, boardHeight - 1));

            for (int i = 0; i < Board.boardWidth; i++)
            {
                AddPiece(new Pawn(PlayerColour.White), new Vector(i, 1));
            }

            for (int i = 0; i < Board.boardWidth; i++)
            {
                AddPiece(new Pawn(PlayerColour.Black), new Vector(i, Board.boardWidth - 2));
            }
        }

        private Piece MakePiece(PieceType type, PlayerColour colour, bool hasMoved = false)
        {
            Piece output = null;
            switch (type)
            {
                case PieceType.Pawn:
                    output = new Pawn(colour, hasMoved);
                    break;
                case PieceType.Knight:
                    output = new Knight(colour, hasMoved);
                    break;
                case PieceType.Bishop:
                    output = new Bishop(colour, hasMoved);
                    break;
                case PieceType.Rook:
                    output = new Rook(colour, hasMoved);
                    break;
                case PieceType.Queen:
                    output = new Queen(colour, hasMoved);
                    break;
                case PieceType.King:
                    output = new King(colour, hasMoved);
                    break;
            }
            return output;
        }


        public void RemoveEnPassante()
        {
            if(enPassantePiece != new Vector(-1, -1))
                ((Pawn)this[enPassantePiece]).VulnurableToEnPassante = false;
                enPassantePiece = new Vector(-1, -1);
        }

        public Piece MovePiece(Vector piecePosition, Vector targetPosition)
        {
            Piece takenPiece = this[targetPosition];

            if(this[piecePosition].Type == PieceType.King)
                    kingPositions[(int)this[piecePosition].Colour] = targetPosition;

            
            switch (targetPosition.SpecialMove)
            {
                case SpecialMove.DoubleFoward:
                    ((Pawn)this[piecePosition]).VulnurableToEnPassante = true;
                    enPassantePiece = targetPosition;
                    break;
                case SpecialMove.CastleLeft:
                    Vector leftRookPos = new Vector(0, piecePosition.Y);
                    MovePiece(leftRookPos, new Vector(targetPosition.X + 1, targetPosition.Y));
                    break;
                case SpecialMove.CastleRight:
                    Vector rightRookPos = new Vector(boardWidth - 1, piecePosition.Y);
                    MovePiece(rightRookPos, new Vector(targetPosition.X - 1, targetPosition.Y));
                    break;
                case SpecialMove.EnPassante:
                    Vector takenPiecePosition = new Vector(targetPosition.X, targetPosition.Y - ((Pawn)this[piecePosition]).Direction);
                    takenPiece = this[takenPiecePosition];
                    this[takenPiecePosition] = null;
                    break;
            }



            this[piecePosition].HasMoved = true;
            this[targetPosition] = this[piecePosition];
            this[piecePosition] = null;
   

            return takenPiece;
        }

        public void AddPiece(Piece piece, Vector position)
        {
            this[position] = piece;
            if (piece.Type == PieceType.King)
                kingPositions[(int)piece.Colour] = position;
        }

        public void GenerateMoves(PlayerColour colour)
        {
            for(int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if(field[i, ii] != null)
                        if(field[i, ii].Colour == colour)
                            field[i, ii].GenerateMoves(this, new Vector(i, ii));
        }

        public void FilterMovesByCheck(PlayerColour colour)
        {
            for (int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if (field[i, ii] != null)
                        if(field[i, ii].Colour == colour)
                           field[i, ii].FilterPositionsByCheck(this, new Vector(i, ii));
        }

        public FENPosition GetFENInformation()
        {
            FENPosition output = new FENPosition();
            string position = "";
            int emptySpaces = 0;


            //The algorithm creates the positions for the FEN
            for (int i = boardHeight; i > 0; i--)
            {
                for(int ii = boardWidth; ii > 0; ii--)
                {
                    Piece piece = this[ii, i];
                    if (piece != null)
                    {
                        if (emptySpaces != 0)
                        {
                            position = position + emptySpaces.ToString();
                            emptySpaces = 0;
                        }

                        string letter = piece.Type.ToString()[0].ToString();
                        if(piece.Colour == PlayerColour.White)
                        {
                            letter = letter.ToUpper();
                        }
                        else
                        {
                            letter = letter.ToLower();
                        }
                        position = position + letter;
                    }
                    else
                    {
                        emptySpaces++;
                    }
                }

                if (i != 0)
                    position = position + "/";
            }
            output.Position = position;

            

            return output;
        }


        public bool IsThereThreat(Vector vector, PlayerColour colour)
        {
            bool output = false;
            for (int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if (field[i, ii] != null)
                        if (field[i, ii].Colour != colour && field[i, ii].AvailableMoves.Contains(vector))
                        {
                            output = true;
                            break;
                        }

            return output;
        }

        public bool IsChecked(PlayerColour colour)
        {    
            return IsThereThreat(kingPositions[(int)colour], colour);
        }


        public Vector GetKingPosition(PlayerColour colour)
        {
            return kingPositions[(int)colour];
        }

        public bool IsTherePossibleMoves(PlayerColour colour)
        {
            for (int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if (field[i, ii] != null)
                        if (field[i, ii].Colour == colour)
                            if (field[i, ii].AvailableMoves.Count > 0)
                            return true;
            return false;
        }

        public Piece this[int x, int y]
        {
            get
            {
                return field[x, y];
            }
            //Set is private so the only way to modify the board is with the methods, so the data in this class and within pieces is accurate 
            private set
            {
                field[x, y] = value;
            }
        }

        public Piece this[Vector vec]
        {
            get
            {
                return this[vec.X, vec.Y];
            }
            private set
            {
                this[vec.X, vec.Y] = value;
            }
        }

        public List<PieceRepresentation> OutputPieces()
        {
            List<PieceRepresentation> pieces = new List<PieceRepresentation>();
            for (int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if (field[i, ii] != null)
                            pieces.Add(new PieceRepresentation(new Vector(i, ii), field[i, ii].Type, field[i, ii].Colour));

            return pieces;
        }

        public static bool operator ==(Board b1, Board b2)
        {
            bool output = true;
            for (int i = 0; i < boardWidth && output; i++)
            {
                for (int ii = 0; ii < boardHeight && output; ii++)
                {
                    if (b1[i, ii] != null && b2[i, ii] != null)
                    {
                        if (b1[i, ii].Colour != b2[i, ii].Colour || b1[i, ii].Type != b2[i, ii].Type)
                        {
                            output = false;
                        }
                    }
                    else if(!(b1[i, ii] == null && b2[i, ii] == null))
                    {
                        output = false;
                    }
                }
            }

            return output;
        }

        public static bool operator !=(Board b1, Board b2)
        {
            return !(b1 == b2);
        }

        public Board Copy()
        {
            Board output = new Board();
            for (int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if (field[i, ii] != null)
                    {
                        Vector pieceCopyPosition = new Vector(i, ii);
                        output.AddPiece(MakePiece(field[i, ii].Type, field[i, ii].Colour, field[i, ii].HasMoved), pieceCopyPosition);
                    }
            return output;
        }

        public int CountPawnsWithEnPassante()
        {
            int output = 0;
            for (int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if (field[i, ii] != null)
                        if (field[i, ii].Type == PieceType.Pawn)
                            if (((Pawn)field[i, ii]).VulnurableToEnPassante)
                                output++;
            return output;
        }
    }
}

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
        static Map<char, PieceType> pieceFENRepresentations;

        public const int boardHeight = 8;
        public const int boardWidth = 8;

        int rule50Counter;

        public int Rule50Counter
        {
            get
            {
                return rule50Counter;
            }
        }

        //Fills the static map 
        static Board()
        {
            pieceFENRepresentations = new Map<char, PieceType>();
            pieceFENRepresentations.Add('p', PieceType.Pawn);
            pieceFENRepresentations.Add('n', PieceType.Knight);
            pieceFENRepresentations.Add('k', PieceType.King);
            pieceFENRepresentations.Add('r', PieceType.Rook);
            pieceFENRepresentations.Add('b', PieceType.Bishop);
            pieceFENRepresentations.Add('q', PieceType.Queen);
        }

        public Board()
        {
            field = new Piece[boardHeight, boardWidth];
            enPassantePiece =Vector.NullVector;
            int numberOfColours = Enum.GetValues(typeof(PlayerColour)).Length;
            kingPositions = new Vector[numberOfColours];
            rule50Counter = 0;
        }

        static public PlayerColour ReverseColour(PlayerColour colour)
        {
            PlayerColour output = PlayerColour.Black;
            if (colour == PlayerColour.Black)
                output = PlayerColour.White;
            return output;
        }

        public static char GetFENNotationFromType(PieceType type)
        {
            return pieceFENRepresentations[type];
        }
        public static PieceType GetTypeFromFENNotation(char c)
        {
            return pieceFENRepresentations[c];
        }

        public void TransformPawn(PlayerColour colour, PieceType piece)
        {
            this[LocatePawnNeedingTransformation(colour)] = MakePiece(piece, colour, true);
        }

        //Returns true if there is a pawn needed to be transformed
        public bool IsPawnTransformationNeeded(PlayerColour colour)
        {
            bool output = false;
            if (LocatePawnNeedingTransformation(colour) != Vector.NullVector)
                output = true;

            return output;
        }

        //Returns the position of the pawn, which needs to be transformed
        private Vector LocatePawnNeedingTransformation(PlayerColour colour)
        {
            int y = 0;
            if (colour == PlayerColour.White)
                y = boardHeight - 1;

            for (int i = 0; i < boardWidth; i++)
                if (field[i, y] != null)
                    if (field[i, y].Type == PieceType.Pawn)
                        return new Vector(i, y);
            return Vector.NullVector;
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

            for (int i = 0; i < boardWidth; i++)
            {
                AddPiece(new Pawn(PlayerColour.White), new Vector(i, 1));
            }

            for (int i = 0; i < boardWidth; i++)
            {
                AddPiece(new Pawn(PlayerColour.Black), new Vector(i, boardWidth - 2));
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
            if(enPassantePiece != Vector.NullVector)
                ((Pawn)this[enPassantePiece]).VulnurableToEnPassante = false;
                enPassantePiece = Vector.NullVector;
        }

        //Moves the piece at given position to the targetPosition
        //Returns the taken piece, (null if no piece was taken)
        public Piece MovePiece(Vector piecePosition, Vector targetPosition)
        {
            Piece takenPiece = this[targetPosition];
            Piece movedPiece = this[piecePosition];
            rule50Counter++;
            if (this[piecePosition].Type == PieceType.King)
                    kingPositions[(int)this[piecePosition].Colour] = targetPosition;

            

            switch (targetPosition.SpecialMove)
            {
                case SpecialMove.DoubleFoward:
                    ((Pawn)this[piecePosition]).VulnurableToEnPassante = true;
                    enPassantePiece = targetPosition;
                    break;
                case SpecialMove.CastleLeft:
                    rule50Counter--;
                    Vector leftRookPos = new Vector(0, piecePosition.Y);
                    MovePiece(leftRookPos, new Vector(targetPosition.X + 1, targetPosition.Y));
                    break;
                case SpecialMove.CastleRight:
                    rule50Counter--;
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

            //Here algorithms determines the 50MoveRule
            if (movedPiece.Type == PieceType.Pawn || takenPiece != null)
                rule50Counter = 0;

            return takenPiece;
        }

        public void AddPiece(Piece piece, Vector position)
        {
            this[position] = piece;
            if (piece.Type == PieceType.King)
                kingPositions[(int)piece.Colour] = position;
        }

        //Generates all the moves in the pieces of given colour,
        //according to chess rules, except checks
        public void GenerateMoves(PlayerColour colour)
        {
            for(int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if(field[i, ii] != null)
                        if(field[i, ii].Colour == colour)
                            field[i, ii].GenerateMoves(this, new Vector(i, ii));
        }

        //Removes all the moves in the pieces of the given player, if they result in a check
        public void FilterMovesByCheck(PlayerColour colour)
        {
            for (int i = 0; i < boardWidth; i++)
                for (int ii = 0; ii < boardHeight; ii++)
                    if (field[i, ii] != null)
                        if(field[i, ii].Colour == colour)
                           field[i, ii].FilterPositionsByCheck(this, new Vector(i, ii));
        }

        //Sets fenPosition specified in the parameters
        public void UploadFENPosition(FENPosition fenPos)
        {
            Array.Clear(field, 0, field.Length);

            int positionCounter = boardHeight * boardWidth - 1;

            foreach (char c in fenPos.Position)
            {
                if (c != '/')
                {
                    if (pieceFENRepresentations.Contains(GetLowerChar(c)))
                    {
                        Vector currentPosition = new Vector(boardWidth - positionCounter % boardWidth - 1, positionCounter / boardHeight);
                        PieceType pieceType = pieceFENRepresentations[GetLowerChar(c)];

                        PlayerColour pieceColour = PlayerColour.Black;
                        if (c < 'a')
                            pieceColour = PlayerColour.White;

                        bool hasMoved = false;
                        switch (pieceType)
                        {
                            case PieceType.Pawn:
                                if (pieceColour == PlayerColour.Black)
                                {
                                    if (currentPosition.Y != boardWidth - 2)
                                        hasMoved = true;
                                }
                                else
                                {
                                    hasMoved = currentPosition.Y != 1;
                                }
                                break;
                            case PieceType.Rook:
                                string checkedLetter = " ";
                                if (currentPosition.X == 0)
                                    checkedLetter = "q";
                                else if (currentPosition.X == boardWidth - 1)
                                    checkedLetter = "k";
                                if (pieceColour == PlayerColour.White)
                                    checkedLetter = checkedLetter.ToUpper();
                                hasMoved = !fenPos.Castling.Contains(checkedLetter);
                                break;
                            case PieceType.King:
                                if (pieceColour == PlayerColour.White)
                                {
                                    hasMoved = (fenPos.Castling.Contains('K') || fenPos.Castling.Contains('Q'));  
                                }
                                else
                                {
                                    hasMoved = (fenPos.Castling.Contains('k') || fenPos.Castling.Contains('q'));
                                }
                                break;
                        }

                        this[currentPosition] = MakePiece(pieceType, pieceColour, hasMoved);
                        positionCounter--;
                    }
                    else
                    {
                        positionCounter -= (int)Char.GetNumericValue((c));
                    }
                }
            }



            if(fenPos.EnPassante.Length == 2)
                enPassantePiece = new Vector(fenPos.EnPassante[0] - 'a', Convert.ToInt32(fenPos.EnPassante[1]));

            rule50Counter = Convert.ToInt32(fenPos.HalfMoveTimer);
        }

        private char GetLowerChar(char c)
        {
            return c.ToString().ToLower()[0];
        }



        //Returns all the information on the board
        public FENPosition GetFENInformation()
        {
            FENPosition output = new FENPosition();
            string position = "";
            int emptySpaces = 0;


            //The algorithm creates the positions for the FEN
            for (int i = boardHeight - 1; i >= 0; i--)
            {
                for(int ii = 0; ii < boardWidth; ii++)
                {
                    Piece piece = this[ii, i];
                    if (piece != null)
                    {
                        if (emptySpaces != 0)
                        {
                            position = position + emptySpaces.ToString();
                            emptySpaces = 0;
                        }

                        string letter = pieceFENRepresentations[piece.Type].ToString();
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

                if (emptySpaces != 0)
                {
                    position = position + emptySpaces.ToString();
                    emptySpaces = 0;
                }

                if (i != 0)
                    position = position + "/";
            }
            output.Position = position;

            output.EnPassante = VectorToStringPosition(enPassantePiece);

            //Now algorithm determines castling rights

            char[] castlingRights = {'-', '-', '-', '-'};
            if(this[0, 7] != null)
                if (!this[0, 7].HasMoved && !this[kingPositions[(int)PlayerColour.White]].HasMoved)
                    castlingRights[0] = 'K';

            if (this[0, 0] != null)
                if (!this[0, 0].HasMoved && !this[kingPositions[(int)PlayerColour.White]].HasMoved)
                    castlingRights[1] = 'Q';

            if (this[7, 7] != null)
                if (!this[7, 7].HasMoved && !this[kingPositions[(int)PlayerColour.Black]].HasMoved)
                    castlingRights[2] = 'k';

            if (this[7, 0] != null)
                if (!this[7, 0].HasMoved && !this[kingPositions[(int)PlayerColour.Black]].HasMoved)
                    castlingRights[3] = 'q';


            output.Castling = new String(castlingRights);

            output.HalfMoveTimer = rule50Counter.ToString();

            return output;
        }

        //Converts vector to string position (ex. a5)
        private string VectorToStringPosition(Vector vector)
        {
            string output = "-";
            if (vector != Vector.NullVector)
                output = ('a' + vector.X.ToString()) + vector.Y.ToString();
            return output;
        }

        //Returns if there is a threat for the player specified by 
        //the colour parameter in the square specified by the vector parameter
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

        //Identifies wheather the player given by the colour parameter is in check
        public bool IsChecked(PlayerColour colour)
        {    
            return IsThereThreat(kingPositions[(int)colour], colour);
        }

        //Returns kings position as a Vector owned by the player specified by the colour parameter
        public Vector GetKingPosition(PlayerColour colour)
        {
            return kingPositions[(int)colour];
        }

        //Checks if there are any available moves for the player given with colour parameter
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

        //Converts the current position on the board to the list with PieceRepresentations
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


        //The method returns the deep copy of the board
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
    }
}

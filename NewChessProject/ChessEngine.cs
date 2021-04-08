using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class ChessEngine
    {

        Process engine;
        string name;
        string position;
        int difficulty;
        int maxumumDepth;
        int maximumTime; //Variable signifies the maximum time the engine can think, if -1, it will take as much time as needed

        public ChessEngine(string address, string name, int difficulty = 20, int maximumTime = -1)
        {
            engine = new Process();
            this.name = name;
            this.maximumTime = maximumTime;
            maxumumDepth = 18;

            engine.EnableRaisingEvents = true;
            engine.StartInfo.UseShellExecute = false;
            engine.StartInfo.RedirectStandardInput = true;
            engine.StartInfo.RedirectStandardOutput = true;
            engine.StartInfo.CreateNoWindow = true;
            engine.StartInfo.FileName = address;

            engine.Start();
            engine.StandardInput.WriteLine("isready");
            engine.StandardInput.WriteLine("uci");
            engine.StandardInput.WriteLine("setoption name Skill Level value " + difficulty.ToString());
        }

        public void UploadPosition(string fenString)
        {
            position = fenString;
        }

        public AiMove GetBestMove()
        {
            engine.StandardInput.WriteLine("position fen " + position);
            engine.StandardInput.WriteLine("go " + GetBoundary());

            string currLine = "";
            do
            {
                currLine = engine.StandardOutput.ReadLine();
                if(currLine == null)
                {
                    continue;
                }
                Console.WriteLine(currLine);
            } while (currLine.Split(' ')[0] != "bestmove");

            if (currLine.Split(' ')[1] == "(none)")
                return new AiMove();

            string move = currLine.Split(' ')[1];

            Vector pieceToMove = new Vector(move[0] - 'a', Convert.ToInt32(move[1].ToString()) - 1);
            Vector placeToMove = new Vector(move[2] - 'a', Convert.ToInt32(move[3].ToString()) - 1);
            PieceType? pawnTransformation = null;
            if(move.Length == 5)
            {
                pawnTransformation = Board.GetTypeFromFENNotation(move[4]);
            }


            AiMove output = new AiMove((pieceToMove, placeToMove), pawnTransformation);

            return output;
        }

        private string GetBoundary()
        {
            if(maximumTime != -1)
            {
                return "movetime " + maximumTime.ToString();
            }
            else
            {
                return "depth " + maxumumDepth.ToString();
            }
        }

        private string TurnVectorToString(Vector vec)
        {
            return ((char)('a' + vec.X)).ToString() + (vec.Y + 1).ToString();
        }

        public int EvaluatePosition()
        {
            engine.StandardInput.WriteLine("position fen " + position);
            engine.StandardInput.WriteLine("go " + GetBoundary());

            string previousLine = "";
            string currLine = "";
            do{
                previousLine = currLine;
                currLine = engine.StandardOutput.ReadLine();
                Console.WriteLine(currLine);
            } while (currLine.Split(' ')[0] != "bestmove") ;

            int posEvaluation = Convert.ToInt32(previousLine.Split(' ')[9]);

            return posEvaluation;
        }
    }
}

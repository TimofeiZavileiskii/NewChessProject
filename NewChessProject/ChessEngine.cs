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
        int maxumumDepth;
        int maximumTime; //Variable signifies the maximum time the engine can think, if -1, it will take as much time as needed
        public ChessEngine(string address, string name, int maximumTime = -1)
        {
            engine = new Process();
            this.name = name;
            maxumumDepth = 20;
            engine.EnableRaisingEvents = true;
            engine.StartInfo.FileName = address;
            engine.Start();
            engine.StandardInput.WriteLine("isready");
            engine.StandardInput.WriteLine("uci");
        }

        public void UploadPosition(string fenString)
        {
            position = fenString;
        }

        public string GetBestMove(string FENPosition)
        {
            engine.StandardInput.WriteLine("position " + FENPosition);
            engine.StandardInput.WriteLine();
            return "";
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
            return ('a' + vec.X).ToString() + vec.Y.ToString();
        }

        public AiMove EvaluateMove(Vector piece, Vector target)
        {
            string moveString = TurnVectorToString(piece) + TurnVectorToString(target);

            engine.StandardInput.WriteLine("position fen " + position + " moves " + moveString);
            engine.StandardInput.WriteLine("go " + GetBoundary());

            string previousLine = engine.StandardOutput.ReadLine();
            string currLine = "";
            do{
                currLine = engine.StandardOutput.ReadLine();
                previousLine = currLine;
            } while (currLine.Split(' ')[0] != "bestmove") ;

            int posEvaluation = Convert.ToInt32(previousLine.Split(' ')[9]);

            return new AiMove(posEvaluation, (piece, target));
        }

        public int EvaluatePosition(string FENPosition)
        {
            return 0;
        }

    }
}

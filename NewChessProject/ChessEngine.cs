﻿using System;
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
            this.maximumTime = maximumTime;
            maxumumDepth = 14;
            engine.EnableRaisingEvents = true;
            engine.StartInfo.UseShellExecute = false;
            engine.StartInfo.RedirectStandardInput = true;
            engine.StartInfo.RedirectStandardOutput = true;
            engine.StartInfo.CreateNoWindow = true;
            engine.StartInfo.FileName = address;
            engine.Start();
            engine.StandardInput.WriteLine("isready");
            engine.StandardInput.WriteLine("uci");
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
            } while (currLine.Split(' ')[0] != "bestmove");

            if (currLine.Split(' ')[1] == "(none)")
                return new AiMove();

            string move = currLine.Split(' ')[1];

            return new AiMove(0, (new Vector(move[0] - 'a', Convert.ToInt32(move[1].ToString()) - 1), new Vector(move[2] - 'a', Convert.ToInt32(move[3].ToString()) - 1)));
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

        public AiMove EvaluateMove(Vector piece, Vector target)
        {
            string moveString = TurnVectorToString(piece) + TurnVectorToString(target);

            engine.StandardInput.WriteLine("position fen " + position + " moves " + moveString);
            engine.StandardInput.WriteLine("go " + GetBoundary());

            string previousLine = "";
            string currLine = "";
            do{
                previousLine = currLine;
                currLine = engine.StandardOutput.ReadLine();
                Console.WriteLine(currLine);
            } while (currLine.Split(' ')[0] != "bestmove") ;

            if (currLine.Split(' ')[1] == "(none)")
                return new AiMove();

            int posEvaluation = Convert.ToInt32(previousLine.Split(' ')[9]);

            return new AiMove(posEvaluation, (piece, target));
        }
    }
}
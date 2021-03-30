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
        public ChessEngine(string address, string name)
        {
            engine = new Process();
            this.name = name;
            engine.EnableRaisingEvents = true;
            engine.StartInfo.FileName = address;
        }

        public string GetBestMove(string FENPosition)
        {
            

            return "";
        }

        public int EvaluatePosition(string FENPosition)
        {
            return 0;
        }

    }
}

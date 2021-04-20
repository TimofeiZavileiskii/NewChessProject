using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class AIPlayer : Player
    {
        const string stockFishAddress = @"stockfish_13_win_x64_bmi2\stockfish_13_win_x64_bmi2";

        ChessEngine engine;
        Thread makeMoveThread;

        Dispatcher mainThreadDispatcher;
        AutoResetEvent hasFinishedWorking = new AutoResetEvent(true);

        public AIPlayer(PlayerColour colour, Game game, int difficulty, int timePerTurn) : base(colour, game)
        {
            engine = new ChessEngine(stockFishAddress, "Stockfish", difficulty, timePerTurn);
            mainThreadDispatcher = Dispatcher.CurrentDispatcher;
        }
        private void MakeMove()
        {
            makeMoveThread = new Thread(() =>
            {
                hasFinishedWorking.WaitOne();
                hasFinishedWorking.Reset();
                engine.UploadPosition(game.GetFENPosition().FENString);
                AiMove chosenMove = engine.GetBestMove();
                hasFinishedWorking.Set();
                mainThreadDispatcher.Invoke(() =>
                {
                    EnterResult result = game.EnterMove(colour, chosenMove.Move.Item1, game.GetAllowedPositions(colour, chosenMove.Move.Item1).Find(x => x == chosenMove.Move.Item2));
                    if (result == EnterResult.WaitForPawnSlection)
                    {
                        game.ChoosePawnTransformation(colour, (PieceType)chosenMove.ChosenPawnTransformation);
                    }            
                });
            });
            makeMoveThread.Start();
        }

        private bool EvaluateDraw()
        {
            bool output = false;
            int assesment = 0;

            hasFinishedWorking.WaitOne();
            hasFinishedWorking.Reset();
            engine.UploadPosition(game.GetFENPosition().FENString);
            assesment = engine.EvaluatePosition();
            hasFinishedWorking.Set();

            int factor = 1;
            if (colour == PlayerColour.Black)
                factor = -1;

            if (assesment * factor < -3)
            {
                output = true;
            }
            return output;
        }

        public override void StartGame(object sender, GameStartEventArgs e)
        {
            if(e.StartingPlayer == Colour)
                MakeMove();
        }

        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            if (e.PlayerToMove == colour)
                MakeMove();
        }

        public override void RequestSend(object sender, RequestMadeEventArgs e)
        {
            if (e.Request.ToWhichPlayer == Colour)
            {
                switch (e.Request.Type)
                {
                    case (RequestType.ProposeTakeback):
                        e.Request.Agreed = true;
                        break;
                    case (RequestType.OfferDraw):
                        e.Request.Agreed = EvaluateDraw();
                        break;
                }
            }
        }

        public override void GameEnded(object sender, GameEndedEventArgs e)
        {
            if (makeMoveThread != null)
            {
                makeMoveThread.Abort();
            }
        }

    }
}

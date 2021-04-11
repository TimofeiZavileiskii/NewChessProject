using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewChessProject
{
    class VisualSettings
    {
        const string visualSettingsFileName = "visualSettings.txt";

        Color blackBoardSquares;
        Color whiteBoardSquares;
        Color defendedHilight;
        Color attackedHilight;
        Color moveHilight;
        Color checkHilight;
        Color selectedPieceHilight;

        public Color BlackBoardSquares
        {
            get {return blackBoardSquares; }
            set { blackBoardSquares = value; }
        }
        public Color WhiteBoardSquares
        {
            get { return whiteBoardSquares; }
            set { whiteBoardSquares = value; }
        }
        public Color DefendedHilight
        {
            get { return defendedHilight; }
            set { defendedHilight = value; }
        }
        public Color AttackedHilight
        {
            get { return attackedHilight; }
            set { attackedHilight = value; }
        }
        public Color MoveHilight
        {
            get { return moveHilight; }
            set { moveHilight = value; }
        }
        public Color CheckHilight
        {
            get { return checkHilight; }
            set { checkHilight = value; }
        }
        public Color SelectedPieceHilight
        {
            get { return selectedPieceHilight; }
            set { selectedPieceHilight = value; }
        }



        public VisualSettings()
        {

        }


        private void ReadVisualSettings()
        {
            /*
            System.IO.StreamReader file = new System.IO.StreamReader(visualSettingsFileName);
            blackBoardSquares = file.ReadLine();
            whiteBoardSquares = file.ReadLine();
            defendedHilight = file.ReadLine();
            attackedHilight = file.ReadLine();
            file.WriteLine(moveHilight);
            file.WriteLine(selectedPieceHilight);
            file.WriteLine(checkHilight);
            file.Close(); */
        }

        private void SetDefaultSettings()
        {
            blackBoardSquares = Color.FromRgb(90, 100, 90);
            whiteBoardSquares = Color.FromRgb(180, 200, 180);
            defendedHilight = Color.FromRgb(30, 220, 50);
            attackedHilight = Color.FromRgb(200, 80, 50);
            moveHilight = Color.FromRgb(200, 200, 50);
            selectedPieceHilight = Color.FromRgb(50, 230, 50);
            checkHilight = Color.FromRgb(180, 40, 40);
        }

        private void WriteVisualSettings()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(visualSettingsFileName);
            file.WriteLine(blackBoardSquares);
            file.WriteLine(whiteBoardSquares);
            file.WriteLine(defendedHilight);
            file.WriteLine(attackedHilight);
            file.WriteLine(moveHilight);
            file.WriteLine(selectedPieceHilight);
            file.WriteLine(checkHilight);
            file.Close();
        }
    }
}

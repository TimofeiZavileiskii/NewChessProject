using System;
using System.IO;
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
    public enum ColourThemes
    {
        Standard,
        ExtraContrast,
        BlueTheme,
        GreenTheme
    }

    public enum PieceTileset
    {
        Classical
    }


    public class VisualSettings
    {
        public event EventHandler SettingUpdated;

        const string visualSettingsFileName = "visualSettings.txt";

        ColourThemes selectedColourTheme;
        PieceTileset pieceTileset;

        List<ColourThemes> possibleColourThemes;
        List<PieceTileset> possiblePieceTilesets;

        Color blackBoardSquares;
        Color whiteBoardSquares;
        Color defendedHilight;
        Color attackedHilight;
        Color moveHilight;
        Color checkHilight;
        Color selectedPieceHilight;

        int movementAnimationSpeed;

        public List<ColourThemes> PossibleColourSchemes
        {
            get { return possibleColourThemes; }
        }
        public List<PieceTileset> PossiblePieceTilesets
        {
            get 
            { return possiblePieceTilesets; }
        }

        public int MovementAnimationSpeed
        {
            get { return movementAnimationSpeed; }
            set 
            { 
                movementAnimationSpeed = value;
                Console.WriteLine(movementAnimationSpeed);
            }
        }
        public ColourThemes ColourTheme
        {
            get { return selectedColourTheme; }
            set { selectedColourTheme = value;
                Console.WriteLine(selectedColourTheme);
            }
        }

        public PieceTileset PieceTileset
        {
            get { return pieceTileset; }
            set { pieceTileset = value;
                Console.WriteLine(pieceTileset);
            }
        }

        public Color BlackBoardSquares
        {
            get {return blackBoardSquares; }
        }
        public Color WhiteBoardSquares
        {
            get { return whiteBoardSquares; }
        }
        public Color DefendedHilight
        {
            get { return defendedHilight; }
        }
        public Color AttackedHilight
        {
            get { return attackedHilight; }
        }
        public Color MoveHilight
        {
            get { return moveHilight; }
        }
        public Color CheckHilight
        {
            get { return checkHilight; }
        }
        public Color SelectedPieceHilight
        {
            get { return selectedPieceHilight; }
        }



        public VisualSettings()
        {
            if (File.Exists(visualSettingsFileName))
                ReadVisualSettings();
            else
                SetDefaultSettings();

            possibleColourThemes = ((ColourThemes[])Enum.GetValues(typeof(ColourThemes))).ToList<ColourThemes>();
            possiblePieceTilesets = ((PieceTileset[])Enum.GetValues(typeof(PieceTileset))).ToList<PieceTileset>();
        }

        private void ReadVisualSettings()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(visualSettingsFileName);
            blackBoardSquares = ConvertStringToColor(file.ReadLine());
            whiteBoardSquares = ConvertStringToColor(file.ReadLine());
            defendedHilight = ConvertStringToColor(file.ReadLine());
            attackedHilight = ConvertStringToColor(file.ReadLine());
            moveHilight = ConvertStringToColor(file.ReadLine());
            selectedPieceHilight = ConvertStringToColor(file.ReadLine());
            checkHilight = ConvertStringToColor(file.ReadLine());

            /*
            selectedColourTheme = file.ReadLine();
            pieceTileset = file.ReadLine();
            movementAnimationSpeed = file.ReadLine();
            */
            file.Close();
        }

        private int ConvertHexadecimalToDenary(string number)
        {
            int output = 0;

            for(int i = 0; i < number.Length; i++)
            {
                char digit = number[i];
                int numericValue;
                if(digit > 'A')
                {
                    numericValue = 10 + digit - 'A';
                }
                else
                {
                    numericValue = (int)Char.GetNumericValue(digit);
                }

                output = (int)Math.Pow(numericValue, i);
            }
            return output;
        }

        private Color ConvertStringToColor(string str)
        {
            int red = ConvertHexadecimalToDenary(str.Substring(1, 2));
            int green = ConvertHexadecimalToDenary(str.Substring(3, 2));
            int blue = ConvertHexadecimalToDenary(str.Substring(5, 2));

            return Color.FromRgb((byte)red, (byte)green, (byte)blue);
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

            selectedColourTheme = ColourThemes.Standard;
            pieceTileset = PieceTileset.Classical;
            movementAnimationSpeed = 8;
            
        }

        public void WriteVisualSettings()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(visualSettingsFileName);

            Console.WriteLine(blackBoardSquares);
            Console.WriteLine(whiteBoardSquares);

            file.WriteLine(blackBoardSquares);
            file.WriteLine(whiteBoardSquares);
            file.WriteLine(defendedHilight);
            file.WriteLine(attackedHilight);
            file.WriteLine(moveHilight);
            file.WriteLine(selectedPieceHilight);
            file.WriteLine(checkHilight);

            file.WriteLine(selectedColourTheme);
            file.WriteLine(pieceTileset);
            file.WriteLine(movementAnimationSpeed);

            file.Close();
        }
    }
}

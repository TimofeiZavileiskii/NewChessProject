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

        Dictionary<ColourThemes, Color> blackBoardSquares;
        Dictionary<ColourThemes, Color> whiteBoardSquares;
        Dictionary<ColourThemes, Color> defendedHilight;
        Dictionary<ColourThemes, Color> attackedHilight;
        Dictionary<ColourThemes, Color> moveHilight;
        Dictionary<ColourThemes, Color> checkHilight;
        Dictionary<ColourThemes, Color> selectedPieceHilight;

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
            get {return blackBoardSquares[selectedColourTheme]; }
        }
        public Color WhiteBoardSquares
        {
            get { return whiteBoardSquares[selectedColourTheme]; }
        }
        public Color DefendedHilight
        {
            get { return defendedHilight[selectedColourTheme]; }
        }
        public Color AttackedHilight
        {
            get { return attackedHilight[selectedColourTheme]; }
        }
        public Color MoveHilight
        {
            get { return moveHilight[selectedColourTheme]; }
        }
        public Color CheckHilight
        {
            get { return checkHilight[selectedColourTheme]; }
        }
        public Color SelectedPieceHilight
        {
            get { return selectedPieceHilight[selectedColourTheme]; }
        }


        private void SetColourThemesArrays()
        {
            blackBoardSquares = new Dictionary<ColourThemes, Color>();
            whiteBoardSquares = new Dictionary<ColourThemes, Color>();
            attackedHilight = new Dictionary<ColourThemes, Color>();
            defendedHilight = new Dictionary<ColourThemes, Color>();
            moveHilight = new Dictionary<ColourThemes, Color>();
            selectedPieceHilight = new Dictionary<ColourThemes, Color>();
            checkHilight = new Dictionary<ColourThemes, Color>();

            blackBoardSquares.Add(ColourThemes.Standard, Color.FromRgb(90, 100, 90));
            whiteBoardSquares.Add(ColourThemes.Standard, Color.FromRgb(180, 200, 180));
            defendedHilight.Add(ColourThemes.Standard, Color.FromRgb(30, 220, 50));
            attackedHilight.Add(ColourThemes.Standard, Color.FromRgb(200, 80, 50));
            moveHilight.Add(ColourThemes.Standard, Color.FromRgb(200, 200, 50));
            selectedPieceHilight.Add(ColourThemes.Standard, Color.FromRgb(50, 230, 50));
            checkHilight.Add(ColourThemes.Standard, Color.FromRgb(180, 40, 40));
        }

        public VisualSettings()
        {
            SetColourThemesArrays();

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

            selectedColourTheme = (ColourThemes)Enum.Parse(typeof(ColourThemes), file.ReadLine());
            pieceTileset = (PieceTileset)Enum.Parse(typeof(PieceTileset), file.ReadLine());
            movementAnimationSpeed = Convert.ToInt32(file.ReadLine());

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

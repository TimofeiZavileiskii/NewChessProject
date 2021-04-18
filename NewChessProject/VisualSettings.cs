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
        ColdTheme,
        WarmTheme
    }

    public enum PieceTileset
    {
        Classical,
        Modern,
    }


    public class VisualSettings
    {
        public event EventHandler SettingsUpdated;

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
            }
        }
        public ColourThemes ColourTheme
        {
            get { return selectedColourTheme; }
            set { selectedColourTheme = value;
                UpdateBoard();
            }
        }

        public PieceTileset PieceTileset
        {
            get { return pieceTileset; }
            set { pieceTileset = value;
                UpdateBoard();
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

        public void UpdateBoard()
        {
            SettingsUpdated?.Invoke(this, EventArgs.Empty);
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
            defendedHilight.Add(ColourThemes.Standard, Color.FromRgb(210, 170, 0));
            attackedHilight.Add(ColourThemes.Standard, Color.FromRgb(170, 60, 40));
            moveHilight.Add(ColourThemes.Standard, Color.FromRgb(30, 220, 50));
            selectedPieceHilight.Add(ColourThemes.Standard, Color.FromRgb(50, 230, 50));
            checkHilight.Add(ColourThemes.Standard, Color.FromRgb(180, 40, 40));

            blackBoardSquares.Add(ColourThemes.ExtraContrast, Color.FromRgb(87, 105, 84));
            whiteBoardSquares.Add(ColourThemes.ExtraContrast, Color.FromRgb(214, 218, 200));
            defendedHilight.Add(ColourThemes.ExtraContrast, Color.FromRgb(220, 180, 0));
            attackedHilight.Add(ColourThemes.ExtraContrast, Color.FromRgb(165, 6, 22));
            moveHilight.Add(ColourThemes.ExtraContrast, Color.FromRgb(88, 223, 78));
            selectedPieceHilight.Add(ColourThemes.ExtraContrast, Color.FromRgb(100, 254, 89));
            checkHilight.Add(ColourThemes.ExtraContrast, Color.FromRgb(160, 35, 35));

            blackBoardSquares.Add(ColourThemes.WarmTheme, Color.FromRgb(188, 144, 56));
            whiteBoardSquares.Add(ColourThemes.WarmTheme, Color.FromRgb(208, 194, 166));
            defendedHilight.Add(ColourThemes.WarmTheme, Color.FromRgb(238, 214, 0));
            attackedHilight.Add(ColourThemes.WarmTheme, Color.FromRgb(232, 70, 30));
            moveHilight.Add(ColourThemes.WarmTheme, Color.FromRgb(142, 240, 44));
            selectedPieceHilight.Add(ColourThemes.WarmTheme, Color.FromRgb(114, 209, 19));
            checkHilight.Add(ColourThemes.WarmTheme, Color.FromRgb(195, 79, 2));

            blackBoardSquares.Add(ColourThemes.ColdTheme, Color.FromRgb(125, 137, 164));
            whiteBoardSquares.Add(ColourThemes.ColdTheme, Color.FromRgb(172, 205, 225));
            defendedHilight.Add(ColourThemes.ColdTheme, Color.FromRgb(221, 242, 117));
            attackedHilight.Add(ColourThemes.ColdTheme, Color.FromRgb(235, 104, 104));
            moveHilight.Add(ColourThemes.ColdTheme, Color.FromRgb(110, 179, 144));
            selectedPieceHilight.Add(ColourThemes.ColdTheme, Color.FromRgb(133, 255, 88));
            checkHilight.Add(ColourThemes.ColdTheme, Color.FromRgb(220, 147, 162));
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


        private void SetDefaultSettings()
        {
            selectedColourTheme = ColourThemes.Standard;
            pieceTileset = PieceTileset.Classical;
            movementAnimationSpeed = 8;
            
        }

        public void WriteVisualSettings()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(visualSettingsFileName);

            file.WriteLine(selectedColourTheme);
            file.WriteLine(pieceTileset);
            file.WriteLine(movementAnimationSpeed);

            file.Close();
        }
    }
}

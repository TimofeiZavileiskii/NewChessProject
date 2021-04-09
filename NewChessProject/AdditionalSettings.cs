using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace NewChessProject
{
    class HumanSettings : PlayerSettings
    {
        bool flipTheBoard;
        bool hilightTakes;
    }

    class AiSettings : PlayerSettings
    {
        int difficulty;
        double maxTimePerTurn;
    }
    class NetworkingSettings : PlayerSettings
    {

    }

    class PlayerSettings
    {
        PlayerColour colour;
    }


    class AdditionalSettings
    {
        StackPanel panel;
        PlayerType blackPlayerType;
        PlayerType whitePlayerType;
        PlayerSettings blackPlayerSettings;
        PlayerSettings whitePlayerSettings;

        const int mainTitleFontSize = 14;
        const int subTitleFontSize = 12;

        public PlayerSettings WhitePlayerSettings
        {
            get
            {
                return whitePlayerSettings;
            }
            set
            {
                whitePlayerSettings = value;
            }
        }

        public PlayerSettings BlackPlayerSettings
        {
            get
            {
                return blackPlayerSettings;
            }
            set
            {
                blackPlayerSettings = value;
            }
        }

        public PlayerType BlackPlayerType
        {
            get
            {
                return blackPlayerType;
            }
            set
            {
                blackPlayerType = value;
                UpdateSettingsPresentation();
            }
        }

        public PlayerType WhitePlayerType
        {
            get
            {
                return whitePlayerType;
            }
            set
            {
                whitePlayerType = value;
                UpdateSettingsPresentation();
            }
        }


        public AdditionalSettings(StackPanel panel)
        {
            this.panel = panel;
        }

        private void UpdateSettingsPresentation() 
        {
            panel.Children.Clear();
            AddTitle("Additional Settings:", mainTitleFontSize);
            if (blackPlayerType == PlayerType.GUIPlayer || whitePlayerType == PlayerType.GUIPlayer)
            {
                AddHumanSettings();
            }

            AddPlayerSettings(PlayerColour.White, whitePlayerType);
            AddPlayerSettings(PlayerColour.Black, blackPlayerType);
        }

        private void AddPlayerSettings(PlayerColour colour, PlayerType type)
        {
            switch (type)
            {
                case PlayerType.AIPlayer:
                    AddAiPlayerSettings(colour);
                    break;
            }
        }


        private void AddHumanSettings()
        {
            AddTitle("Players' interface", subTitleFontSize);
            AddCheckBox("Flip board:");
            AddCheckBox("Hilight threats:");
        }

        private void AddAiPlayerSettings(PlayerColour playersColour)
        {
            AddTitle("Ai settings (colour " + playersColour.ToString() + ")", subTitleFontSize);
            AddTextBox("Difficulty:");
            AddTextBox("Time spend per turn:");
        }

        private void AddNetworkingSettings(PlayerColour playersColour)
        {
             
        }

        private void AddCheckBox(string name)
        {
            WrapPanel wp = new WrapPanel();
            Label label = CreateLabel(name, 10);
            CheckBox checkBox = CreateCheckBox();
            wp.Children.Add(label);
            wp.Children.Add(checkBox);
            panel.Children.Add(wp);
        }

        private void AddTextBox(string name)
        {
            WrapPanel wp = new WrapPanel();
            Label label = CreateLabel(name, 10);
            TextBox textBox = CreateTextBox();
            wp.Children.Add(label);
            wp.Children.Add(textBox);
            panel.Children.Add(wp);
        }
     
        private void AddTitle(string text, int fontSize)
        {
            panel.Children.Add(CreateLabel(text, fontSize));
        }

        private TextBox CreateTextBox()
        {
            return new TextBox();
        }

        private CheckBox CreateCheckBox()
        {
            return new CheckBox();
        }
        private Label CreateLabel(string text, int fontSize)
        {
            Label label = new Label();
            label.Content = text;
            label.FontSize = fontSize;

            panel.Children.Add(label);
            return label;
        }


    }
}

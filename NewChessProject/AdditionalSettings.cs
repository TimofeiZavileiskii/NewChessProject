﻿using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace NewChessProject
{
    class HumanSettings : PlayerSettings
    {
        bool flipBoard;
        bool hilightTakes;

        public HumanSettings()
        {
            flipBoard = true;
            hilightTakes = false;
        }

        public bool FlipBoard
        {
            get
            {
                return flipBoard;
            }
            set
            {
                flipBoard = value;
                Console.WriteLine(value);
            }
        }
        public bool HighlightThreats
        {
            get
            {
                return hilightTakes;
            }
            set
            {
                hilightTakes = value;
                Console.WriteLine(value);
            }
        }
    }

    class AiSettings : PlayerSettings
    {
        int difficulty;
        int maxTimePerTurn;

        public AiSettings()
        {
            difficulty = 8;
        }

        public int Difficulty
        {
            get
            {
                return difficulty;
            }
            set
            {
                difficulty = value;
                Console.WriteLine(value);
            }
        }
        public int MaxTimePerTurn
        {
            get
            {
                return maxTimePerTurn;
            }
            set
            {
                maxTimePerTurn = value;
                Console.WriteLine(value);
            }
        }
    }

    class NetworkingSettings : PlayerSettings
    {

    }

    abstract class PlayerSettings
    {

    }


    class AdditionalSettings
    {
        StackPanel panel;
        PlayerType blackPlayerType;
        PlayerType whitePlayerType;
        PlayerSettings blackPlayerSettings;
        PlayerSettings whitePlayerSettings;
        PlayerSettings[] playerSettings;
        HumanSettings generalSettings;

        const int mainTitleFontSize = 14;
        const int subtitleFontSize = 13;
        const int normalFontSize = 12;

        delegate void IntSetter(int input);
        delegate void BoolSetter(bool input);
        delegate void DoubleSetter(double input);


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

        public HumanSettings GeneralSettings
        {
            get
            {
                return generalSettings;
            }
            set
            {
                generalSettings = value;
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
                playerSettings[(int)PlayerColour.Black] = blackPlayerSettings;
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
                playerSettings[(int)PlayerColour.White] = whitePlayerSettings;
            }
        }

        public PlayerSettings[] PlayerSettings
        {
            get
            {
                return playerSettings;
            }
        }


        public AdditionalSettings(StackPanel panel)
        {
            this.panel = panel;
            playerSettings = new PlayerSettings[Enum.GetValues(typeof(PlayerColour)).Length];
        }

        private void UpdateSettingsPresentation() 
        {
            panel.Children.Clear();
            AddTitle("Additional Settings:", mainTitleFontSize);
            if (blackPlayerType == PlayerType.GUIPlayer || whitePlayerType == PlayerType.GUIPlayer)
            {
                generalSettings = AddHumanSettings(generalSettings);
            }

            whitePlayerSettings = AddPlayerSettings(PlayerColour.White, whitePlayerType, whitePlayerSettings);
            blackPlayerSettings = AddPlayerSettings(PlayerColour.Black, blackPlayerType, blackPlayerSettings);
        }

        private PlayerSettings AddPlayerSettings(PlayerColour colour, PlayerType type, PlayerSettings settings)
        {
            switch (type)
            {
                case PlayerType.AIPlayer:
                    settings = AddAiPlayerSettings(settings, colour);
                    break;
            }

            return settings;
        }


        private HumanSettings AddHumanSettings(HumanSettings playerSettings)
        {
            if (playerSettings == null)
                playerSettings = new HumanSettings();

            AddTitle("Players' interface", subtitleFontSize);
            Label flipBoardLbl = CreateLabel("Flip board:", normalFontSize);
            CheckBox flipBoardCb = CreateCheckBox();
            AddWrapPanel(flipBoardLbl, flipBoardCb);

            flipBoardCb.DataContext = playerSettings;
            Binding flipBoardBind = new Binding("FlipBoard");
            flipBoardBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            flipBoardCb.SetBinding(CheckBox.IsCheckedProperty, flipBoardBind);

            Label highlightThreatsLbl = CreateLabel("Highlight threats:", normalFontSize);
            CheckBox highlightThreatsCb = CreateCheckBox();
            AddWrapPanel(highlightThreatsLbl, highlightThreatsCb);

            highlightThreatsCb.DataContext = playerSettings;
            Binding hilightThreatsBind = new Binding("HighlightThreats");
            hilightThreatsBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            highlightThreatsCb.SetBinding(CheckBox.IsCheckedProperty, hilightThreatsBind);

            return playerSettings;
        }

        private PlayerSettings AddAiPlayerSettings(PlayerSettings aiSettings, PlayerColour colour)
        {
            if(aiSettings == null)
                aiSettings = new AiSettings();

            AddTitle("Ai settings (" + colour.ToString() + ")", subtitleFontSize);

            Label difficultyLbl = CreateLabel("Difficulty:", normalFontSize);
            TextBox difficultTb = CreateTextBox();
            AddWrapPanel(difficultyLbl, difficultTb);

            difficultTb.DataContext = aiSettings;
            Binding difficultyBind = new Binding("Difficulty");
            difficultyBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            difficultTb.SetBinding(TextBox.TextProperty, difficultyBind);

            Label maxTimeLbl = CreateLabel("Time spend per turn:", normalFontSize);
            TextBox maxTimeTb = CreateTextBox();
            AddWrapPanel(maxTimeLbl, maxTimeTb);

            maxTimeTb.DataContext = aiSettings;
            Binding maxTimeBind = new Binding("MaxTimePerTurn");
            maxTimeBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            maxTimeTb.SetBinding(TextBox.TextProperty, maxTimeBind);

            return aiSettings;
        }

        private void AddNetworkingSettings(PlayerColour playersColour)
        {
             
        }


        private void AddWrapPanel(UIElement ui1, UIElement ui2)
        {
            WrapPanel wp = new WrapPanel();
            wp.Children.Add(ui1);
            wp.Children.Add(ui2);
            panel.Children.Add(wp);
        }

     
        private void AddTitle(string text, int fontSize)
        {
            panel.Children.Add(CreateLabel(text, fontSize));
        }

        private TextBox CreateTextBox()
        {
            TextBox tb = new TextBox();
            tb.MinWidth = 200;
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
            return label;
        }


    }
}

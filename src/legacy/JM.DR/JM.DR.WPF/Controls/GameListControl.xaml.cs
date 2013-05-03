using System;
using System.Collections.Generic;
using System.Windows.Controls;

using OM = JM.DR.ObjectModel;

namespace JM.DR.WPF.Controls
{
    public partial class GameListControl : UserControl
    {
        public GameListControl()
        {
            InitializeComponent();
        }

        public OM.Game SelectedGame
        {
            get
            {
                return gameListBox.SelectedItem as OM.Game;
            }
        }

        public void SetGameList(List<OM.Game> games)
        {
            gameListBox.ItemsSource = games;
            gameListBox.SelectedIndex = 0;
        }
    }
}
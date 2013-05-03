using System;
using System.Collections.Generic;
using System.Windows.Controls;

using OM = JM.DR.ObjectModel;

namespace JM.DR.WPF.Controls
{
    public partial class CharacterListControl : UserControl
    {
        public CharacterListControl()
        {
            InitializeComponent();
        }

        public OM.Character SelectedCharacter
        {
            get
            {
                return characterListBox.SelectedItem as OM.Character;
            }
        }


        public void SetCharacterList(List<OM.Character> characters)
        {
            characterListBox.ItemsSource = characters;
            characterListBox.SelectedIndex = 0;
        }
    }
}
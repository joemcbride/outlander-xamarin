using System;
using System.Windows.Controls;

namespace JM.DR.WPF.Controls
{
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        public String Account
        {
            get
            {
                return AccountTextBox.Text;
            }
            set
            {
                AccountTextBox.Text = value;
            }
        }

        public String Password
        {
            get
            {
                return PasswordTextBox.Password;
            }
            set
            {
                PasswordTextBox.Password = value;
            }
        }
    }
}
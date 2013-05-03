using System;
using System.ComponentModel;
using System.Windows;

using OM = JM.DR.ObjectModel;

namespace JM.DR.WPF
{
    public partial class LoginWindow : Window
    {
        private Step m_Step;

        private static String m_AuthenticateText = "Enter your account and password information.";
        private static String m_SelectGameText = "Select a game...";
        private static String m_SelectCharacterText = "Select a character...";

        public LoginWindow()
        {
            InitializeComponent();

            m_Step = Step.Authenticate;

            PreviousButton.Click += new RoutedEventHandler(PreviousButton_Click);
            NextButton.Click += new RoutedEventHandler(NextButton_Click);
            CancelButton.Click += new RoutedEventHandler(CancelButton_Click);

            this.Closing += new CancelEventHandler(LoginWindow_Closing);
        }

        private void ToggleControls()
        {
            switch (m_Step)
            {
                case Step.Authenticate:
                    {
                        loginControl.Visibility = Visibility.Visible;
                        gameListControl.Visibility = Visibility.Collapsed;
                        characterListControl.Visibility = Visibility.Collapsed;
                    }
                    break;
                case Step.Game:
                    {
                        loginControl.Visibility = Visibility.Collapsed;
                        gameListControl.Visibility = Visibility.Visible;
                        characterListControl.Visibility = Visibility.Collapsed;
                    }
                    break;
                case Step.Character:
                    {
                        loginControl.Visibility = Visibility.Collapsed;
                        gameListControl.Visibility = Visibility.Collapsed;
                        characterListControl.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        private void LoginWindow_Closing(Object sender, CancelEventArgs e)
        {
            OM.Application.Instance.DisconnectAuthenticationServer();
        }

        private void PreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            switch (m_Step)
            {
                case Step.Authenticate:
                    break;
                case Step.Game:
                    statusText.Text = m_AuthenticateText;
                    m_Step = Step.Authenticate;
                    PreviousButton.IsEnabled = false;
                    break;
                case Step.Character:
                    statusText.Text = m_SelectGameText;
                    m_Step = Step.Game;
                    break;
            }

            ToggleControls();
        }

        private void NextButton_Click(Object sender, RoutedEventArgs e)
        {
            String message = String.Empty;

            switch (m_Step)
            {
                case Step.Authenticate:

                    if (String.IsNullOrEmpty(loginControl.Account) || string.IsNullOrEmpty(loginControl.Password))
                    {
                        message = "You must provide an account name and password!";
                        break;
                    }

                    statusText.Text = "Authenticating...";

                    if (OM.Application.Instance.Authenticate(loginControl.Account, loginControl.Password))
                    {
                        message = m_SelectGameText;

                        gameListControl.SetGameList(OM.Application.Instance.Games);

                        loginControl.Visibility = Visibility.Collapsed;
                        gameListControl.Visibility = Visibility.Visible;

                        PreviousButton.IsEnabled = true;

                        m_Step = Step.Game;
                    }
                    else
                    {
                        message = "Login failed.";
                    }
                    break;
                case Step.Game:

                    if (gameListControl.SelectedGame != null)
                    {
                        characterListControl.SetCharacterList(OM.Application.Instance.GetCharacters(gameListControl.SelectedGame));

                        m_Step = Step.Character;

                        message = m_SelectCharacterText;
                    }

                    break;
                case Step.Character:

                    if (characterListControl.SelectedCharacter != null)
                    {
                        if (OM.Application.Instance.ConnectAs(characterListControl.SelectedCharacter))
                        {
                            this.Close();
                            return;
                        }
                        else
                        {
                            message = "Unable to connect!";
                        }
                    }

                    break;
            }

            ToggleControls();

            statusText.Text = message;
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private enum Step
        {
            Authenticate,
            Game,
            Character
        };
    }
}
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

using OM = JM.DR.ObjectModel;

namespace JM.DR.WPF
{
    public partial class MainWindow : Window
    {
        private delegate void AddGameDataHandler(String data);

        private Timer m_Timer;
        private OM.GameServer m_GameServer;

        public MainWindow()
        {
            InitializeComponent();

            GameRichTextBox.Document.Blocks.Clear();

            CommandTextBox.KeyUp += new KeyEventHandler(CommandTextBox_KeyUp);

            OM.Application.Instance.PropertyChanged += new PropertyChangedEventHandler(Instance_PropertyChanged);

            this.Closing += new CancelEventHandler(MainWindow_Closing);
        }

        private void MainWindow_Closing(Object sender, CancelEventArgs e)
        {
            if (m_Timer != null)
            {
                m_Timer.Dispose();
            }

            if (m_GameServer != null)
            {
                m_GameServer.SendCommand("quit");
                m_GameServer.Close();
            }
        }

        private void Instance_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ConnectionToken", StringComparison.InvariantCultureIgnoreCase))
            {
                OM.SGE.ConnectionToken token = OM.Application.Instance.ConnectionToken;

                if (!token.IsValid)
                    return;

                AddGameData("Connecting to game ...");

                AddGameData(String.Format("Host: {0} Port: {1} Key: {2}\nIsValid: {3}",
                    token.GameHost, token.GamePort, token.Key, token.IsValid));

                m_GameServer = new OM.GameServer();

                m_GameServer.Connect(token, null);

                m_Timer = new Timer(Timer_Elapsed, m_GameServer, 0, 1000);
            }
        }

        private void CommandTextBox_KeyUp(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.AddGameData(">" + CommandTextBox.Text);

                m_GameServer.SendCommand(CommandTextBox.Text);

                CommandTextBox.Text = String.Empty;
            }
        }

        private void Connect_Click(Object sender, RoutedEventArgs e)
        {
            LoginWindow lw = new LoginWindow();
            lw.Owner = this;
            lw.ShowDialog();
        }

        private void AddGameData(String data)
        {
            Paragraph p = new Paragraph(new Run(data.Trim(new Char[] { '\r', '\n' })));

            p.Margin = new Thickness(0);

            GameRichTextBox.Document.Blocks.Add(p);

            if (!viewer.IsFocused)
            {
                viewer.ScrollToEnd();
            }
        }

        private void Timer_Elapsed(Object objectState)
        {
            OM.GameServer server = objectState as OM.GameServer;

            if (server == null)
                return;

            String data = server.Poll();

            if (!String.IsNullOrEmpty(data))
            {
                Delegate d = new AddGameDataHandler(AddGameData);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, d, data);
            }
        }
    }
}
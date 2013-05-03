using System;
using System.Collections.Generic;
using System.ComponentModel;

using JM.DR.ObjectModel.SGE;

namespace JM.DR.ObjectModel
{
    public sealed class Application : IDisposable, INotifyPropertyChanged
    {
        #region Private Members
        private AuthenticationServer m_AuthenticationServer;
        private List<Game> m_Games;
        private Profile m_CurrentProfile;

        private ConnectionToken m_ConnectionToken;
        private Boolean m_AuthenticationServerConnected;

        private event PropertyChangedEventHandler m_PropertyChanged;

        private static Application m_Instance;
        #endregion

        #region Constructors
        static Application()
        {
            m_Instance = new Application();
        }

        private Application()
        {
            m_Games = new List<Game>();

            m_CurrentProfile = new Profile("Default");

            m_AuthenticationServerConnected = false;
        }
        #endregion

        #region Public Properties
        public static Application Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public ConnectionToken ConnectionToken
        {
            get
            {
                return m_ConnectionToken;
            }
            set
            {
                m_ConnectionToken = value;

                FirePropertyChanged("ConnectionToken");
            }
        }

        public Profile CurrentProfile
        {
            get
            {
                return m_CurrentProfile;
            }
            set
            {
                m_CurrentProfile = value;

                FirePropertyChanged("CurrentProfile");
            }
        }

        public List<Game> Games
        {
            get
            {
                return m_Games;
            }
        }
        #endregion

        #region Public Methods
        public Boolean Authenticate(String account, String password)
        {
            if (m_AuthenticationServer != null)
            {
                m_AuthenticationServer.Dispose();
            }

            m_AuthenticationServer = new AuthenticationServer("eaccess.play.net", 7900);

            if (m_AuthenticationServer.Authenticate(account, password))
            {
                m_AuthenticationServerConnected = true;

                m_Games = m_AuthenticationServer.GetGameList();

                return true;
            }

            return false;
        }

        public List<Character> GetCharacters(Game game)
        {
            if (!m_AuthenticationServerConnected)
            {
                return null;
            }

            return m_AuthenticationServer.GetCharacterList(game);
        }

        public Boolean ConnectAs(Character character)
        {
            this.ConnectionToken = m_AuthenticationServer.ChooseCharacter(character);

            return this.ConnectionToken.IsValid;
        }

        public void DisconnectAuthenticationServer()
        {
            if (m_AuthenticationServer == null)
                return;

            m_AuthenticationServer.Close();
        }

        public void SendCommand(String command)
        {
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { m_PropertyChanged += value; }
            remove { m_PropertyChanged -= value; }
        }

        private void FirePropertyChanged(String property)
        {
            if (m_PropertyChanged != null)
            {
                m_PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_AuthenticationServer.Dispose();
        }

        #endregion
    }
}
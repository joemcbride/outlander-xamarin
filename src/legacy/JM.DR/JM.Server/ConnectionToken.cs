using System;
using System.Collections.Generic;
using System.Text;

namespace JM.Server
{
    public sealed class ConnectionToken
    {
        private String m_GameHost;
        private Int32 m_GamePort;
        private String m_Key;

        public ConnectionToken()
            : this(String.Empty, 0, String.Empty)
        { }

        public ConnectionToken(String gameHost, Int32 gamePort, String key)
        {
            m_GameHost = gameHost;
            m_GamePort = gamePort;
            m_Key = key;
        }

        public String GameHost
        {
            get
            {
                return m_GameHost;
            }
            set
            {
                m_GameHost = value;
            }
        }

        public Int32 GamePort
        {
            get
            {
                return m_GamePort;
            }
            set
            {
                m_GamePort = value;
            }
        }

        public String Key
        {
            get
            {
                return m_Key;
            }
            set
            {
                m_Key = value;
            }
        }
    }
}

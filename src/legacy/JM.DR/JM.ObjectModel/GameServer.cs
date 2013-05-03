using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

using JM.DR.ObjectModel.SGE;

namespace JM.DR.ObjectModel
{
    public sealed class GameServer
    {
        // remote end point for socket
        private IPEndPoint m_RemoteEndPoint;

        // TCP/IP  socket
        private SimpleSocket m_Socket;

        private readonly String m_StormFrontVersion = "1.0.1.26";
        //private readonly String m_WizardVersion = "2.04";
        private readonly Int32 m_BufferSize = 3072;

        public GameServer()
        {
            m_Socket = new SimpleSocket(m_BufferSize);
        }

        public void Connect(ConnectionToken token, AsyncCallback receiveCallback)
        {
            Debug.WriteLine("GameServer - Connect Called.");

            // create the remote endpoint for the socket.
            m_RemoteEndPoint = new IPEndPoint(Dns.GetHostEntry(token.GameHost).AddressList[0], token.GamePort);

            m_Socket.Connect(m_RemoteEndPoint);

            String connectionString = String.Format("{0}\r\n/FE:STORMFRONT /VERSION:{1} /P:{2} /XML\r\n", token.Key, m_StormFrontVersion, Environment.OSVersion.Platform);

            //String connectionString = String.Format("{0}\r\n/FE:JAVA", token.Key);

            m_Socket.Send(connectionString);
        }

        public void SendCommand(String command)
        {
            m_Socket.Send(command);
        }

        public String Poll()
        {
            if (m_Socket == null || m_Socket.Connected == false)
            {
                return String.Empty;
            }

            return m_Socket.Receive();
        }

        public void Close()
        {
            m_Socket.Close();
        }
    }
}

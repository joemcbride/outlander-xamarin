using System;
using System.Diagnostics;
using Pathfinder.Core.Authentication;

namespace Pathfinder.Core
{
    public sealed class GameServer
    {
        private readonly ISimpleSocket _socket;

        private const string StormFrontVersion = "1.0.1.26";
        private const int BufferSize = 3072;

        public GameServer()
        {
            _socket = new SimpleSocket(BufferSize);
        }

        public void Connect(ConnectionToken token)
        {
            Debug.WriteLine("GameServer - Connect Called.");

            _socket.Connect(token.GameHost, token.GamePort);

            var connectionString = String.Format("{0}\r\n/FE:STORMFRONT /VERSION:{1} /P:{2} /XML\r\n", token.Key, StormFrontVersion, Environment.OSVersion.Platform);

            //String connectionString = String.Format("{0}\r\n/FE:JAVA", token.Key);

            _socket.Send(connectionString);
        }

        public void SendCommand(String command)
        {
            _socket.Send(command);
        }

        public string Poll()
        {
            if (_socket == null || _socket.Connected == false)
            {
                return String.Empty;
            }

            return _socket.Receive();
        }

        public void Close()
        {
            _socket.Close();
        }
    }
}

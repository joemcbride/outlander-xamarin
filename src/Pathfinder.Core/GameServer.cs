using System;
using System.Diagnostics;
using Pathfinder.Core.Authentication;
using System.Text;

namespace Pathfinder.Core
{
    public sealed class GameServer
    {
        private readonly ISimpleSocket _socket;

        private const string StormFrontVersion = "1.0.1.26";
        private const int BufferSize = 3072;

		private StringBuilder _builder = new StringBuilder();

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
			if (_socket == null || _socket.Connected == false)
				return;

            _socket.Send(command);
        }

        public string Poll()
        {
            if (_socket == null || _socket.Connected == false)
            {
				Debug.WriteLine("Not connnected");

                return String.Empty;
            }

			var data = _socket.Receive();

			if(_socket.LastError != null) {
				Debug.WriteLine(_socket.LastError.Message);
			}

			return data;

//			_builder.Append(data);
//
//			var value = _builder.ToString();
//
//			var idx = value.LastIndexOf("\n");
//			if (idx == -1)
//				idx = 0;
//			else
//				_builder.Clear();
//
//			return value.Substring(0, value.Length - idx);
        }

        public void Close()
        {
            _socket.Close();
        }
    }
}

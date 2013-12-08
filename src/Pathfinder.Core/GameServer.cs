using System;
using System.Diagnostics;
using Pathfinder.Core.Authentication;
using System.Text;

namespace Pathfinder.Core
{
    public sealed class GameServer
    {
        private readonly ISimpleSocket _socket;
		private readonly ILog _logger;

        private const string StormFrontVersion = "1.0.1.26";
		private const int BufferSize = 5120;

		private StringBuilder _builder = new StringBuilder();

		public GameServer()
			: this(new SimpleSocket(BufferSize), new SimpleFileLogger())
        {
        }

		public GameServer(ISimpleSocket socket, ILog logger)
		{
			_socket = socket;
			_logger = logger;
		}

        public void Connect(ConnectionToken token)
        {
            Debug.WriteLine("GameServer - Connect Called.");

            _socket.Connect(token.GameHost, token.GamePort);

			var connectionString = String.Format("{0}\r\n/FE:STORMFRONT /VERSION:{1} /P:{2} /XML\r\n", token.Key, StormFrontVersion, Environment.OSVersion.Platform);

			//var connectionString = String.Format("{0}\r\n/FE:JAVA", token.Key);

			Debug.WriteLine(connectionString);

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

			if (_socket.LastError == null && !string.IsNullOrWhiteSpace(data)) {
				_logger.Info(data);
			}

			if(_socket.LastError != null) {
				_logger.Error(_socket.LastError);
			}

			_builder.Append(data);

			var value = _builder.ToString();
			var returnData = string.Empty;

			var idx = value.LastIndexOf("\n");
			if(idx > -1){
				idx += 1;
				returnData = value.Substring(0, idx);
				_builder.Remove(0, idx);
			}

			return returnData;
        }

        public void Close()
        {
            _socket.Close();
        }
    }
}

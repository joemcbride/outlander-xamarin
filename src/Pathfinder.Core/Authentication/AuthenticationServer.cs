using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace Pathfinder.Core.Authentication
{
	public interface IAuthenticationServer : IDisposable
	{
		void Connect(string host, int port);
		bool Authenticate(string account, string password);
		IEnumerable<Game> GetGameList();
		IEnumerable<Character> GetCharacterList(string gameCode);
		ConnectionToken ChooseCharacter(string characterId);
		void Close();
	}

	public sealed class AuthenticationServer : IAuthenticationServer
    {
        private readonly ISimpleSocket _socket;
        private readonly ICharacterParser _characterParser;
		private readonly IAuthGameParser _gameParser;
        private readonly IConnectionTokenParser _connectionTokenParser;
        private readonly IPasswordHashProvider _passwordHashProvider;
		private readonly ILog _logger;

		public AuthenticationServer(ILog logger)
        {
			_logger = logger;
            _socket = new SimpleSocket();
            _characterParser = new CharacterParser();
			_gameParser = new AuthGameParser();
            _connectionTokenParser = new ConnectionTokenParser();
            _passwordHashProvider = new PasswordHashProvider();
        }

		public void Connect(string host, int port)
        {
			try
			{
            	_socket.Connect(host, port);
			}
			catch(Exception exc)
			{
				_logger.Error(exc);
			}
        }

        public bool Authenticate(string account, string password)
        {
            if (String.IsNullOrEmpty(account) || String.IsNullOrEmpty(password))
            {
                return false;
            }

            try
            {
                if (!this._socket.Connected == true)
                {
					throw new InvalidOperationException("Not connected");
                }

                var passwordToken = _socket.SendAndReceive("K");

                Debug.WriteLine(string.Format("Echoed passwordToken = {0}", passwordToken));

                var hashedPassword = _passwordHashProvider.Hash(passwordToken, password);

                var message = String.Format("A\t{0}\t{1}", account, hashedPassword);

                var result = _socket.SendAndReceive(message);

                Debug.WriteLine(string.Format("Rec: {0}", result));

                if (result.Contains("KEY"))
                {
                    return true;
                }
            }
            catch (SocketException se)
            {
                Debug.WriteLine("SocketException : {0}", se.ToString());
				_logger.Error(se);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unexpected exception : {0}", e.ToString());
				_logger.Error(e);
            }

            return false;
        }

        public IEnumerable<Game> GetGameList()
        {
            var result = _socket.SendAndReceive("M");

            Debug.WriteLine("GameList", result);

            return _gameParser.Parse(result);
        }

        public IEnumerable<Character> GetCharacterList(string gameCode)
        {
            String response;

            response = _socket.SendAndReceive(String.Format("F\t{0}", gameCode));
            //Debug.WriteLine("F: " + response);

            response = _socket.SendAndReceive(String.Format("G\t{0}", gameCode));
            //Debug.WriteLine("G: " + response);

            //response = SendCommand(String.Format("P\t{0}", gameCode));
            //Debug.WriteLine("P: " + response);

            response = _socket.SendAndReceive("C");
            Debug.WriteLine("C: " + response);

            return _characterParser.Parse(response);
        }

        public ConnectionToken ChooseCharacter(string characterId)
        {
            var connectString = String.Format("L\t{0}\tPLAY\r\n", characterId);

            var data = _socket.SendAndReceive(connectString);

            Debug.WriteLine(data);

            return _connectionTokenParser.Parse(data);
        }

        public void Close()
        {
            if (_socket == null)
                return;

            try
            {
                _socket.Close();
            }
            catch (SocketException se)
            {
                Debug.WriteLine("SocketException : {0}", se.ToString());
				_logger.Error(se);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unexpected exception : {0}", e.ToString());
				_logger.Error(e);
            }
        }

        public void Dispose()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);

                this.Close();
            }
            catch (SocketException se)
            {
                Debug.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}

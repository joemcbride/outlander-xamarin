using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core
{
	public interface IGameServer
	{
		IGameState GameState { get; }

		void Connect(ConnectionToken token);
		void Disconnect();
		void SendCommand(string command);

		ConnectionToken Authenticate(string game, string account, string password, string character);
	}

	public class SimpleGameServer : IGameServer
	{
		const string StormFrontVersion = "1.0.1.26";
		const string ConnectionStringTemplate = "{0}\r\n/FE:STORMFRONT /VERSION:{1} /P:{2} /XML\r\n";

		private readonly IGameState _gameState;
		private readonly ILog _logger;
		private readonly IServiceLocator _locator;
		private readonly StringBuilder _builder = new StringBuilder();
		private IAsyncSocket _asyncSocket;

		public SimpleGameServer(IGameState gameState, ILog logger, IServiceLocator locator)
		{
			_gameState = gameState;
			_locator = locator;
			_logger = logger;
		}

		public IGameState GameState {
			get {
				return _gameState;
			}
		}

		public void Connect(ConnectionToken token)
		{
			var connectionString = String.Format(ConnectionStringTemplate, token.Key, StormFrontVersion, Environment.OSVersion.Platform);

			_asyncSocket = _locator.Get<IAsyncSocket>();
			_asyncSocket.ReceiveMessage += HandleReceiveMessage;
			_asyncSocket.Connect(token.GameHost, token.GamePort);
			_asyncSocket.SendMessage(connectionString);
		}

		public void Disconnect()
		{
			_asyncSocket.Disconnect();
		}

		public void SendCommand(string command)
		{
			_asyncSocket.SendMessage(command + "\r\n");
		}

		private void HandleReceiveMessage(string message)
		{
			_builder.Append(message);

			var value = _builder.ToString();

			var idx = value.LastIndexOf("\n");
			if (idx > -1) {
				idx += 1;
				var data = value.Substring(0, idx);
				_builder.Remove(0, idx);

				_logger.Info(data);

				try {
					_gameState.Read(data);
				}
				catch(Exception exc){
					_logger.Error(exc);
				}
			}
		}

		public ConnectionToken Authenticate(string game, string account, string password, string character)
		{
			using (var authServer = _locator.Get<IAuthenticationServer>())
			{
				authServer.Connect("eaccess.play.net", 7900);

				var authenticated = authServer.Authenticate(account, password);

				if (!authenticated) {
					_logger.Warn("Authentication Failed.");
					return null;
				}

				var gameList = authServer.GetGameList();
				gameList
					.Select(g => g.Code + ", " + g.Name)
					.Apply(_logger.Info);

				var characters = authServer.GetCharacterList(game).ToList();
				characters
					.Select(c => c.CharacterId + ", " + c.Name)
					.Apply(_logger.Info);

				var characterId = characters
					.Where(x => x.Name.ToLower() == character.ToLower())
					.Select(x => x.CharacterId)
					.FirstOrDefault();

				ConnectionToken token = null;
				if (!string.IsNullOrWhiteSpace(characterId)) {
					token = authServer.ChooseCharacter(characterId);
				}

				return token;
			}
		}
	}
}

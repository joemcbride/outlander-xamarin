using System;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Tests
{
	public class StubGameServer : IGameServer
	{
		private StubGameState _gameState;

		public StubGameServer(StubGameState gameState)
		{
			_gameState = gameState;
		}

		public string LastCommand { get; private set; }

		public void Connect(ConnectionToken token)
		{
			throw new NotImplementedException();
		}

		public void Disconnect()
		{
			throw new NotImplementedException();
		}

		public void SendCommand(string command)
		{
			LastCommand = command;
			_gameState.FireTextLog(command);
		}

		public ConnectionToken Authenticate(string game, string account, string password, string character)
		{
			throw new NotImplementedException();
		}

		public IGameState GameState {
			get {
				return _gameState;
			}
		}
	}

	public class StubGameState : IGameState
	{
		private IDictionary<string, string> _state = new Dictionary<string, string>();

		public StubGameState()
		{
			TagTracker = new DataTracker<IEnumerable<Tag>>();
		}

		public string LastReadData { get; set; }
		public string LastEcho { get; set; }

		public string Get(string key)
		{
			return _state[key];
		}

		public void Set(string key, string value)
		{
			_state[key] = value;
		}

		public void Read(string data)
		{
			LastReadData = data;
		}

		public void Echo(string text)
		{
			LastEcho = text;
			FireTextLog(text);
		}

		public ISimpleDictionary<string, string> GlobalVars()
		{
			return new SimpleDictionary<string, string>(_state);
		}

		public void FireTextLog(string data)
		{
			var ev = TextLog;
			if(ev != null){
				ev(data);
			}
		}

		public event TextLogHandler TextLog;

		public DataTracker<string> TextTracker { get; set; }
		public DataTracker<IEnumerable<Tag>> TagTracker { get; set; }
		public Action<IEnumerable<Tag>> Tags { get; set; }
		public Action<SkillExp> Exp { get; set; }
	}
}

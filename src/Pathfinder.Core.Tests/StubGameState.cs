using System;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Tests
{
	public class StubGameState : IGameState
	{
		private IDictionary<string, string> _state = new Dictionary<string, string>();

		public string LastReadData { get; set; }

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

		public Action<string> TextLog { get; set; }
		public Action<RoundtimeTag> Roundtime { get;set; }
	}
}

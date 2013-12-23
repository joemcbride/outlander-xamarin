using System;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Tests;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client.Tests
{
	public class StubScriptLoader : IScriptLoader
	{
		private ISimpleDictionary<string, string> _data = new SimpleDictionary<string, string>();

		public void AddData(string name, string data)
		{
			_data[name] = data;
		}

		public bool CanLoad(string name)
		{
			return _data.HasKey(name);
		}

		public string Load(string name)
		{
			return _data[name];
		}
	}
}

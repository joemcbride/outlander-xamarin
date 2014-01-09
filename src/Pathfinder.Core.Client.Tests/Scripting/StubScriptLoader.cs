using System;
using System.Linq;
using NUnit.Framework;
using Outlander.Core.Tests;
using Outlander.Core.Client.Scripting;

namespace Outlander.Core.Client.Tests
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

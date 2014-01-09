using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Outlander.Core.Tests;

namespace Outlander.Core.Client.Tests
{
	public class StubCommandProcessor : ICommandProcessor
	{
		public string LastEval { get; set; }
		public string LastProcess { get; set; }

		public string Eval(string command, ScriptContext context = null)
		{
			return command;
		}

		public Task Process(string command, ScriptContext context = null, bool echo = true)
		{
			LastProcess = command;
			return null;
		}

		public Task Echo(string command, ScriptContext context = null)
		{
			throw new NotImplementedException();
		}

		public Task Parse(string command, ScriptContext context = null)
		{
			throw new NotImplementedException();
		}
	}
}

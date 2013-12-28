using System;
using System.Threading;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class VariableReplacerTester
	{
		private VariableReplacer theReplacer;
		private ScriptContext theContext;
		private InMemoryServiceLocator theLocator;
		private StubGameState theGameState;
		private SimpleDictionary<string, string> theVars;

		[SetUp]
		public void SetUp()
		{
			theReplacer = new VariableReplacer();
			theLocator = new InMemoryServiceLocator();
			theGameState = new StubGameState();
			theVars = new SimpleDictionary<string, string>();
			theContext = new ScriptContext("1", "Name", CancellationToken.None, theLocator, theVars);

			theLocator.Add<IGameState>(theGameState);
		}

		[Test]
		public void replaces_local_vars()
		{
			const string scriptline = "play %1 %2";
			const string expected = "play ditty masterful";

			theVars.Set("1", "ditty");
			theVars.Set("2", "masterful");

			var result = theReplacer.Replace(scriptline, theContext);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void replaces_global_vars()
		{
			const string scriptline = "play $play.song $play.style";
			const string expected = "play ditty masterful";

			theGameState.Set("play.song", "ditty");
			theGameState.Set("play.style", "masterful");

			var result = theReplacer.Replace(scriptline, theContext);

			Assert.AreEqual(expected, result);
		}
	}
}

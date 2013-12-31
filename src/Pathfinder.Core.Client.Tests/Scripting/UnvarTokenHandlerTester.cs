using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class UnvarTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private ISimpleDictionary<string, string> theLocalVars;
		private InMemoryScriptLog theScriptLog;
		private InMemoryServiceLocator theServices;
		private UnVarTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);
			theScriptLog = new InMemoryScriptLog();
			theLocalVars = new SimpleDictionary<string, string>();

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IScriptLog>(theScriptLog);

			theScriptContext = new ScriptContext("1", "unvar", CancellationToken.None, theServices, theLocalVars);
			theHandler = new UnVarTokenHandler();
		}

		[Test]
		public void removes_the_local_variable()
		{
			var token = new Token
			{
				Type = "unvar",
				Text = "unvar something",
				Value = "something"
			};

			theLocalVars.Set("something", "value");

			var task = theHandler.Execute(theScriptContext, token);

			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.False(theLocalVars.HasKey("something"));
		}
	}
}

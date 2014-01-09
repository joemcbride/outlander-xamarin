using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Outlander.Core.Tests;

namespace Outlander.Core.Client.Tests
{
	[TestFixture]
	public class GotoTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private StubCommandProcessor theCommandProcessor;
		private InMemoryScriptLog theScriptLog;
		private InMemoryServiceLocator theServices;
		private GotoTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);
			theScriptLog = new InMemoryScriptLog();
			theCommandProcessor = new StubCommandProcessor();

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IScriptLog>(theScriptLog);
			theServices.Add<ICommandProcessor>(theCommandProcessor);

			theScriptContext = new ScriptContext("1", "gototoken", CancellationToken.None, theServices, null);
			theScriptContext.DebugLevel = 5;
			theHandler = new GotoTokenHandler();
		}

		[Test]
		public void sets_the_goto_label()
		{
			var token = new Token
			{
				Type = "goto",
				Text = "goto somewhere",
				Value = "somewhere"
			};

			var task = theHandler.Execute(theScriptContext, token);

			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual(token.Value, task.Result.Goto);
		}
	}
}

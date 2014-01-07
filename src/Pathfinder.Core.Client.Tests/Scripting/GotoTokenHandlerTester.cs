using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class GotoTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private ICommandProcessor theCommandProcessor;
		private InMemoryScriptLog theScriptLog;
		private InMemoryServiceLocator theServices;
		private GotoTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);
			theScriptLog = new InMemoryScriptLog();

			//TODO: add stub command processor

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IScriptLog>(theScriptLog);
			theServices.Add<ICommandProcessor>(theCommandProcessor);

			theScriptContext = new ScriptContext("1", "gototoken", CancellationToken.None, theServices, null);
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

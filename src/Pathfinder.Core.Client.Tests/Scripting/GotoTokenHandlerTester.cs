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
		private InMemoryServiceLocator theServices;
		private GotoTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);

			theScriptContext = new ScriptContext("gototoken", CancellationToken.None, theServices, null);
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

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual(token.Value, task.Result.Goto);
		}
	}
	
}

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pathfinder.Core.Tests;
using Pathfinder.Core.Client.Scripting;
using Outlander.Core.Client;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class WaitForReTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private GameStream theGameStream;
		private ScriptContext theScriptContext;
		private InMemoryScriptLog theLog;
		private InMemoryServiceLocator theServices;
		private WaitForReTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);
			theGameStream = new GameStream(theGameState);

			theGameState.Set(ComponentKeys.Roundtime, "0");

			theLog = new InMemoryScriptLog();

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IScriptLog>(theLog);

			theScriptContext = new ScriptContext("1", "waitfor", CancellationToken.None, theServices, null);
			theScriptContext.DebugLevel = 5;

			theHandler = new WaitForReTokenHandler(theGameState, theGameStream);
		}

		[Test]
		public void waits_for_value()
		{
			var token = new Token
			{
				Type = "waitforre",
				Text = "waitforre You take a step back|Now what did the|I could not find",
				Value = "You take a step back|Now what did the|I could not find"
			};

			var task = theHandler.Execute(theScriptContext, token);

			theGameState.FireTextLog("[Derelict Road, Darkling Wood]");

			Assert.False(task.IsCompleted);

			theGameState.FireTextLog("I could not find what you were referring to.");

			Assert.True(task.IsCompleted);
			Assert.AreEqual("waitforre You take a step back|Now what did the|I could not find\n", theLog.Builder.ToString());
		}
	}
}

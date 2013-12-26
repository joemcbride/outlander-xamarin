using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pathfinder.Core.Tests;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class WaitForTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private InMemoryScriptLog theLog;
		private InMemoryServiceLocator theServices;
		private WaitForTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);

			theGameState.Set(ComponentKeys.Roundtime, "0");

			theLog = new InMemoryScriptLog();

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IScriptLog>(theLog);

			theScriptContext = new ScriptContext("waitfor", CancellationToken.None, theServices, null);

			theHandler = new WaitForTokenHandler();
		}

		[Test]
		public void waits_for_value()
		{
			var token = new Token
			{
				Type = "waitfor",
				Text = "waitfor You finish playing",
				Value = "You finish playing"
			};

			var task = theHandler.Execute(theScriptContext, token);

			theGameState.FireTextLog("[Derelict Road, Darkling Wood]");

			Assert.False(task.IsCompleted);

			theGameState.FireTextLog("You finish playing your zills.");

			Assert.True(task.IsCompleted);
			Assert.AreEqual("waitfor You finish playing\n", theLog.Builder.ToString());
		}
	}
}

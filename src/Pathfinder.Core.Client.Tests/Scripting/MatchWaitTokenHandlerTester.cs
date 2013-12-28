using System;
using NUnit.Framework;
using Pathfinder.Core.Tests;
using System.Threading;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class MatchWaitTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private InMemoryScriptLog theLog;
		private InMemoryServiceLocator theServices;
		private MatchWaitTokenHandler theHandler;

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

			theScriptContext = new ScriptContext("1", "matchwait", CancellationToken.None, theServices, null);

			theHandler = new MatchWaitTokenHandler(theGameState);
		}

		[Test]
		public void waits_for_match()
		{
			var token = new Token
			{
				Type = "matchwait",
				Text = "matchwait 5",
				Value = "5"
			};

			theScriptContext.MatchWait.Add(MatchToken.For("Derelict Road", "one", false));

			var task = theHandler.Execute(theScriptContext, token);

			theGameState.FireTextLog("You finish playing your zills.");

			Assert.False(task.IsCompleted);

			theGameState.FireTextLog("[Derelict Road, Darkling Wood]");

			Assert.True(task.IsCompleted);
			Assert.AreEqual("one", task.Result.Goto);
			Assert.AreEqual("matchwait\nmatch goto one\n", theLog.Builder.ToString());
		}

		[Test]
		public void waits_for_match_regex()
		{
			var token = new Token
			{
				Type = "matchwait",
				Text = "matchwait 5",
				Value = "5"
			};

			theScriptContext.MatchWait.Add(MatchToken.For("Another Road|Derelict Road", "one", true));

			var task = theHandler.Execute(theScriptContext, token);

			theGameState.FireTextLog("You finish playing your zills.");

			Assert.False(task.IsCompleted);

			theGameState.FireTextLog("[Derelict Road, Darkling Wood]");

			Assert.True(task.IsCompleted);
			Assert.AreEqual("one", task.Result.Goto);
			Assert.AreEqual("matchwait\nmatch goto one\n", theLog.Builder.ToString());
		}

		[Test]
		public void waits_for_match_second_regex()
		{
			var token = new Token
			{
				Type = "matchwait",
				Text = "matchwait 5",
				Value = "5"
			};

			theScriptContext.MatchWait.Add(MatchToken.For("one|two|three", "one", true));
			theScriptContext.MatchWait.Add(MatchToken.For("Another Road|Derelict Road", "two.match", true));

			var task = theHandler.Execute(theScriptContext, token);

			theGameState.FireTextLog("You finish playing your zills.");

			Assert.False(task.IsCompleted);

			theGameState.FireTextLog("[Derelict Road, Darkling Wood]");

			Assert.True(task.IsCompleted);
			Assert.AreEqual("two.match", task.Result.Goto);
			Assert.AreEqual("matchwait\nmatch goto two.match\n", theLog.Builder.ToString());
		}
	}
}


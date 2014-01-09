using System;
using System.Threading;
using NUnit.Framework;
using Outlander.Core;
using Outlander.Core.Client;
using Outlander.Core.Tests;
using Outlander.Core.Client.Tests;

namespace Outlander.Core.Client.Tests
{
	[TestFixture]
	public class ContainsReTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private StubCommandProcessor theCommandProcessor;
		private InMemoryScriptLog theScriptLog;
		private InMemoryServiceLocator theServices;
		private ContainsReTokenHandler theHandler;

		private ISimpleDictionary<string, string> theLocalVars;

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

			theLocalVars = new SimpleDictionary<string, string>();

			theScriptContext = new ScriptContext("1", "containsre", CancellationToken.None, theServices, theLocalVars);
			theScriptContext.DebugLevel = 5;
			theHandler = new ContainsReTokenHandler();
		}

		[Test]
		public void sets_the_variable_non_match()
		{
			var token = new Token
			{
				Type = "containsre",
				Text = "containsre result somewhere something",
				Value = "result somewhere something"
			};

			var task = theHandler.Execute(theScriptContext, token);

			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("False", theLocalVars.Get("result"));
		}

		[Test]
		public void sets_the_variable_match()
		{
			var token = new Token
			{
				Type = "containsre",
				Text = "containsre result \"somewhere something\" something",
				Value = "result \"somewhere something\" something"
			};

			var task = theHandler.Execute(theScriptContext, token);

			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("True", theLocalVars.Get("result"));
		}

		[Test]
		public void sets_the_variable_match_regex()
		{
			var token = new Token
			{
				Type = "containsre",
				Text = "containsre result \"somewhere something\" something",
				Value = "result \"Circle: 5\" \"Circle: (d+)\""
			};

			var task = theHandler.Execute(theScriptContext, token);

			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("True", theLocalVars.Get("result"));
		}
	}
}

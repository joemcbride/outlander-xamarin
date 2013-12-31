using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{

	[TestFixture]
	public class HasVarTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private ISimpleDictionary<string, string> theLocalVars;
		private InMemoryScriptLog theScriptLog;
		private InMemoryServiceLocator theServices;
		private HasVarTokenHandler theHandler;

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

			theScriptContext = new ScriptContext("1", "hasvar", CancellationToken.None, theServices, theLocalVars);
			theHandler = new HasVarTokenHandler();
		}

		[Test]
		public void sets_newvar_to_true()
		{
			var token = new Token
			{
				Type = "hasvar",
				Text = "hasvar something newvar",
				Value = "something newvar"
			};

			theLocalVars.Set("something", "value");

			var task = theHandler.Execute(theScriptContext, token);

			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("True", theLocalVars.Get("newvar"));
		}

		[Test]
		public void sets_newvar_to_false()
		{
			var token = new Token
			{
				Type = "hasvar",
				Text = "hasvar something newvar",
				Value = "something newvar"
			};

			var task = theHandler.Execute(theScriptContext, token);

			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("False", theLocalVars.Get("newvar"));
		}
	}
}

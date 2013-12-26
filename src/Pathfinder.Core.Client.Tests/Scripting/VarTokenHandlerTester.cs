using System;
using System.Threading;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class VarTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ISimpleDictionary<string, string> theLocalVars;
		private IVariableReplacer theReplacer;
		private ScriptContext theScriptContext;
		private InMemoryScriptLog theScriptLog;
		private InMemoryServiceLocator theServices;
		private VarTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);
			theScriptLog = new InMemoryScriptLog();

			theReplacer = new VariableReplacer();

			theLocalVars = new SimpleDictionary<string, string>();

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IGameState>(theGameState);
			theServices.Add<IScriptLog>(theScriptLog);
			theServices.Add<IVariableReplacer>(theReplacer);

			theScriptContext = new ScriptContext("vartoken", CancellationToken.None, theServices, theLocalVars);
			theHandler = new VarTokenHandler();
		}

		[Test]
		public void sets_the_variable()
		{
			var token = new Token
			{
				Type = "var",
				Text = "var something value",
				Value = "something value"
			};

			var task = theHandler.Execute(theScriptContext, token);
			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("value", theLocalVars.Get("something"));
		}

		[Test]
		public void sets_the_variable_with_spaces()
		{
			var token = new Token
			{
				Type = "var",
				Text = "var something.other value with spaces",
				Value = "something.other value with spaces"
			};

			var task = theHandler.Execute(theScriptContext, token);
			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("value with spaces", theLocalVars.Get("something.other"));
		}

		[Test]
		public void sets_the_variable_with_underscore()
		{
			var token = new Token
			{
				Type = "var",
				Text = "var something_other value with spaces",
				Value = "something_other value with spaces"
			};

			var task = theHandler.Execute(theScriptContext, token);
			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("value with spaces", theLocalVars.Get("something_other"));
		}

		[Test]
		public void sets_the_variable_with_local_var()
		{
			var token = new Token
			{
				Type = "var",
				Text = "var something_other %1",
				Value = "something_other %1"
			};

			theLocalVars.Set("1", "some arg");

			var task = theHandler.Execute(theScriptContext, token);
			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("some arg", theLocalVars.Get("something_other"));
		}

		[Test]
		public void sets_the_variable_with_global_var()
		{
			var token = new Token
			{
				Type = "var",
				Text = "var something_other $something.other",
				Value = "something_other $something.other"
			};

			theGameState.Set("something.other", "some arg");

			var task = theHandler.Execute(theScriptContext, token);
			task.Wait();

			Assert.IsTrue(task.IsCompleted);
			Assert.AreEqual("some arg", theLocalVars.Get("something_other"));
		}
	}
}


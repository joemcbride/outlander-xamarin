using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Outlander.Core.Tests;
using Outlander.Core.Client.Scripting;

namespace Outlander.Core.Client.Tests
{
	[TestFixture]
	public class PauseTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private InMemoryServiceLocator theServices;
		private InMemoryScriptLog theLogger;
		private CancellationTokenSource theCancelSource;
		private PauseTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);
			theLogger = new InMemoryScriptLog();
			theCancelSource = new CancellationTokenSource();

			theGameState.Set(ComponentKeys.Roundtime, "0");

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IGameState>(theGameState);
			theServices.Add<IScriptLog>(theLogger);

			theScriptContext = new ScriptContext("1", "pausetoken", theCancelSource.Token, theServices, null);

			theHandler = new PauseTokenHandler();
		}

		[Test]
		public void pauses_for_given_time()
		{
			var token = new Token
			{
				Type = "pause",
				Text = "pause 0.5",
				Value = "0.5"
			};

			var task = theHandler.Execute(theScriptContext, token);

			var start = DateTime.Now;
			Task.WaitAll(task);
			var end = DateTime.Now;

			var diff = end - start;

			Assert.IsTrue(task.IsCompleted);
			Assert.LessOrEqual(diff, TimeSpan.FromSeconds(0.5));
		}

		[Test]
		public void pauses_by_default_for_1_second()
		{
			var token = new Token
			{
				Type = "pause",
				Text = "pause",
				Value = ""
			};

			var task = theHandler.Execute(theScriptContext, token);

			var start = DateTime.Now;
			Task.WaitAll(task);
			var end = DateTime.Now;

			var diff = end - start;

			Assert.IsTrue(task.IsCompleted);
			Assert.LessOrEqual(diff, TimeSpan.FromSeconds(1.0));
		}

		[Test]
		public void handles_canceled()
		{
			var token = new Token
			{
				Type = "pause",
				Text = "pause",
				Value = ""
			};

			theCancelSource.Cancel();

			var task = theHandler.Execute(theScriptContext, token);

			var start = DateTime.Now;
			try
			{
				Task.WaitAll(task);
			}
			catch(AggregateException)
			{}
			var end = DateTime.Now;

			var diff = end - start;

			Assert.IsTrue(task.IsCompleted);
			Assert.IsTrue(task.IsCanceled);
			Assert.LessOrEqual(diff, TimeSpan.FromSeconds(1.0));
		}
	}
}

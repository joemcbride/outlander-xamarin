using System;
using System.Threading;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class SendCommandTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private InMemoryServiceLocator theServices;
		private InMemoryScriptLog theLogger;
		private SendCommandTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);
			theLogger = new InMemoryScriptLog();

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IGameState>(theGameState);
			theServices.Add<IScriptLog>(theLogger);
			theServices.Add<IVariableReplacer>(new VariableReplacer());
			theServices.Add<ICommandProcessor>(new CommandProcessor(theServices, theServices.Get<IVariableReplacer>(), theLogger));

			theScriptContext = new ScriptContext("1", "sendcommand", CancellationToken.None,  theServices, null);

			theHandler = new SendCommandTokenHandler();
		}

		[Test]
		public void sends_command_to_processor()
		{
			var token = new Token
			{
				Type = "put",
				Text = "put play $play.song $play.style",
				Value = "play $play.song $play.style"
			};

			theGameState.Set("play.song", "ditty");
			theGameState.Set("play.style", "masterful");

			var task = theHandler.Execute(theScriptContext, token);

			Assert.True(task.IsCompleted);
			Assert.AreEqual("play ditty masterful\n", theLogger.Builder.ToString());
			Assert.AreEqual("play ditty masterful", theGameServer.LastCommand);
		}
	}
}


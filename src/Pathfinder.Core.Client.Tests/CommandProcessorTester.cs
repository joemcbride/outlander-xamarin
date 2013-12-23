using System;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Client.Scripting;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class CommandProcessorTester
	{
		private InMemoryServiceLocator theServices;
		private StubScriptRunner theRunner;
		private StubGameState theGameState;
		private StubGameServer theGameServer;
		private InMemoryScriptLog theScriptLog;
		private VariableReplacer theVariableReplacer;
		private CommandProcessor theProcessor;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);

			theVariableReplacer = new VariableReplacer();

			theScriptLog = new InMemoryScriptLog();

			theRunner = new StubScriptRunner();
			theServices = new InMemoryServiceLocator();

			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IGameState>(theGameState);
			theServices.Add<IScriptRunner>(theRunner);
			theServices.Add<IVariableReplacer>(theVariableReplacer);

			theProcessor = new CommandProcessor(theServices, theVariableReplacer, theScriptLog);
		}

		[Test]
		public void processes_start_script_command()
		{
			const string line = ".myscript one two \"three four\"";

			var task = theProcessor.Process(line);
			task.Wait();

			Assert.NotNull(theRunner.RunToken);
			Assert.AreEqual("myscript", theRunner.RunToken.Name);
			Assert.True(new string[]{ "one", "two", "three four"}.SequenceEqual(theRunner.RunToken.Args));
		}

		[Test]
		public void processes_script_abort_command()
		{
			const string line = "#script abort myscript";

			var task = theProcessor.Process(line);
			task.Wait();

			Assert.NotNull(theRunner.StopToken);
			Assert.AreEqual("myscript", theRunner.StopToken.Name);
		}

		[Test]
		public void processes_script_pause_command()
		{
			const string line = "#script pause myscript";

			var task = theProcessor.Process(line);
			task.Wait();

			Assert.NotNull(theRunner.PauseToken);
			Assert.AreEqual("myscript", theRunner.PauseToken.Name);
		}

		[Test]
		public void processes_script_resume_command()
		{
			const string line = "#script resume myscript";

			var task = theProcessor.Process(line);
			task.Wait();

			Assert.NotNull(theRunner.ResumeToken);
			Assert.AreEqual("myscript", theRunner.ResumeToken.Name);
		}

		[Test]
		public void processes_normal_command()
		{
			theProcessor.Process("collect rock");

			Assert.AreEqual("collect rock", theGameServer.LastCommand);
		}

		[Test]
		public void processes_command_with_vars()
		{
			theGameState.Set("play.song", "ditty");
			theGameState.Set("play.style", "masterful");

			theProcessor.Process("play $play.song $play.style");

			Assert.AreEqual("play ditty masterful", theGameServer.LastCommand);
		}
	}
}

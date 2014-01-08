using System;
using System.Threading;
using NUnit.Framework;
using Pathfinder.Core.Tests;
using Pathfinder.Core.Client.Scripting;
using Outlander.Core.Client;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class IfTokenHandlerTester
	{
		private StubGameServer theGameServer;
		private StubGameState theGameState;
		private ScriptContext theScriptContext;
		private IVariableReplacer theReplacer;
		private ISimpleDictionary<string, string> theLocalVars;
		private ICommandProcessor theCommandProcessor;
		private IGameStream theGameStream;
		private InMemoryScriptLog theLog;
		private InMemoryServiceLocator theServices;
		private IfTokenHandler theHandler;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theGameServer = new StubGameServer(theGameState);

			theGameState.Set(ComponentKeys.Roundtime, "0");

			theLog = new InMemoryScriptLog();

			theReplacer = new VariableReplacer();
			theGameStream = new GameStream(theGameState);

			theServices = new InMemoryServiceLocator();
			theServices.Add<IGameServer>(theGameServer);
			theServices.Add<IGameState>(theGameState);
			theServices.Add<IScriptLog>(theLog);
			theServices.Add<IVariableReplacer>(theReplacer);
			theServices.Add<IIfBlockExecuter>(
				new IfBlockExecuter(
					new WaitForTokenHandler(theGameState, theGameStream),
					new WaitForReTokenHandler(theGameState, theGameStream),
					new MatchWaitTokenHandler(theGameState, theGameStream)
				));
			theServices.Add<IGameStream>(theGameStream);

			theCommandProcessor = new CommandProcessor(theServices, theReplacer, theLog);
			theServices.Add<ICommandProcessor>(theCommandProcessor);

			theLocalVars = new SimpleDictionary<string, string>();

			theScriptContext = new ScriptContext("1", "if", CancellationToken.None, theServices, theLocalVars);
			theScriptContext.DebugLevel = 5;

			theHandler = new IfTokenHandler();
		}

		[Test]
		public void waits_for_match()
		{
			var token = new IfToken
			{
				Type = "if",
				Text = "if ($Outdoorsmanship.LearningRate >= %maxexp) then goto END",
				Blocks = new IfBlocks
				{
					IfEval = "($Outdoorsmanship.LearningRate >= %maxexp)",
					IfBlock = "goto END"
				}
			};

			theGameState.Set("Outdoorsmanship.LearningRate", "10");
			theLocalVars.Set("maxexp", "5");

			var task = theHandler.Execute(theScriptContext, token);

			Assert.True(task.IsCompleted);
			Assert.AreEqual("if (10 >= 5)\nif result true\ngoto END\n", theLog.Builder.ToString());
			Assert.AreEqual("END", task.Result.Goto);
		}

		[Test]
		public void waits_for_match2()
		{
			var token = new IfToken
			{
				Type = "if",
				Text = "if (\"%snapcast\" == \"OFF\") then goto END\nelse goto elsewhere",
				Blocks = new IfBlocks
				{
					IfEval = "(\"%snapcast\" == \"OFF\")",
					IfBlock = "goto END",
					ElseBlock = "goto elsewhere"
				}
			};

			theLocalVars.Set("snapcast", "ON");

			var task = theHandler.Execute(theScriptContext, token);

			Assert.True(task.IsCompleted);
			Assert.AreEqual("if (\"ON\" == \"OFF\")\nif result false\ngoto elsewhere\n", theLog.Builder.ToString());
			Assert.AreEqual("elsewhere", task.Result.Goto);
		}

		[Test]
		public void if_elseif_else()
		{
			var token = new IfToken
			{
				Type = "if",
				Text = "Cast:\n\tif (\"%snapCast\" = \"OFF\") then\n\t{\n\t\twaitfor fully prepared\n\t\techo line 1\n\t} else if (\"something1\" = \"another\") then {\n\t\tpause 1\n\t\techo line 2\n\t} else {\n\t\tpause 7\n\t\techo line 3\n\t}\n\tput cast\n\tmatchre ManaCheck You strain\n\tmatchre ExpCheck snap\n\tmatchwait 4\n\tgoto ExpCheck\n\nExpCheck:\n\tpause 1",
				Blocks = new IfBlocks
				{
					IfEval = "(\"%snapCast\" == \"OFF\")",
					IfBlock = "{\n\t\tpause 0.3\n\t\techo line 1\n\t}",
					ElseIf = "(\"something1\" == \"another\")",
					ElseIfBlock = "{\n\t\tpause 0.2\n\t\techo line 2\n\t}",
					ElseBlock = "{\n\t\tpause 0.1\n\t\techo snapcast %snapCast\n\t}"
				}
			};

			const string expected = "if (\"ON\" == \"OFF\")\nif result false\nif (\"something1\" == \"another\")\nif result false\npausing for 0.1 seconds\necho snapcast ON\n\n";

			theLocalVars.Set("snapCast", "ON");

			var task = theHandler.Execute(theScriptContext, token);

			Assert.True(task.IsCompleted);
			Assert.AreEqual(expected, theLog.Builder.ToString());
		}

		[Test]
		public void throws_exception()
		{
			var token = new IfToken
			{
				Type = "if",
				Text = "if (\"%snapcast\" = \"OFF\") then goto END"
			};

			theLocalVars.Set("snapcast", "OFF");

			var task = theHandler.Execute(theScriptContext, token);

			Assert.Throws<AggregateException>(() => task.Wait());
		}
	}
}

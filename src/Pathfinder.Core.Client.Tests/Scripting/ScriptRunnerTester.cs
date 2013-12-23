using System;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Tests;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class ScriptRunnerTester
	{
		private ScriptRunner theRunner;
		private StubScript theScript;
		private StubScriptLoader theLoader;
		private InMemoryScriptLog theLogger;
		private InMemoryServiceLocator theServices;

		[SetUp]
		public void SetUp()
		{
			theScript = new StubScript();
			theLoader = new StubScriptLoader();
			theLogger = new InMemoryScriptLog();
			theServices = new InMemoryServiceLocator();
			theRunner = new ScriptRunner(theServices, theLoader, theLogger);

			theRunner.Create = () => theScript;
		}

		[Test]
		public void runs_the_script()
		{
			const string scriptData = "start:\nput %0\npause 1\ngoto start";

			theLoader.AddData("myscript", scriptData);

			var token = new ScriptToken();
			token.Name = "myscript";
			token.Args = new string[]{ "one", "two", "three four" };

			var task = theRunner.Run(token);
			task.Wait();

			Assert.AreEqual(token.Name, theScript.Name);
			Assert.True(token.Args.SequenceEqual(theScript.Args));
			Assert.AreEqual(scriptData, theScript.Script);
		}

		[Test]
		public void does_not_try_to_run_script_if_cannot_load()
		{
			var token = new ScriptToken();
			token.Name = "myscript";
			token.Args = new string[]{ "one", "two", "three four" };

			var task = theRunner.Run(token);

			Assert.IsNull(task);
			Assert.IsNull(theScript.Script);
			Assert.IsNull(theScript.Args);
		}

		[Test]
		public void run_integrated_script()
		{
			theRunner.Create = theRunner.DefaultCreate;

			var theLogger = new InMemoryScriptLog();
			theServices.Add<IScriptLog>(theLogger);

			const string scriptData = "start:\ngoto end\nend:";

			theLoader.AddData("myscript", scriptData);

			var token = new ScriptToken();
			token.Name = "myscript";
			token.Args = new string[]{ "one", "two", "three four" };

			var task = theRunner.Run(token);

			task.ContinueWith(t => {
				Assert.AreEqual("passing label: start\nmoving to label end\npassing label: end\n", theLogger.Builder.ToString());
				Assert.AreEqual(0, theRunner.Scripts().Count());
			});
		}
	}
}

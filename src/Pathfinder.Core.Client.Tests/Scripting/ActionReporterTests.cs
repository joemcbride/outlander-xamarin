using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pathfinder.Core.Client;
using Pathfinder.Core.Client.Scripting;
using Pathfinder.Core.Tests;
using Pathfinder.Core.Client.Tests;

namespace Outlander.Core.Client.Tests
{
	[TestFixture]
	public class ActionReporterTests
	{
		private ActionReporter theReporter;
		private InMemoryScriptLog theLog;
		private InMemoryServiceLocator theServices;
		private StubIfBlockExecuter theExectuter;

		[SetUp]
		public void SetUp()
		{
			theLog = new InMemoryScriptLog();
			theServices = new InMemoryServiceLocator();
			theExectuter = new StubIfBlockExecuter();
			theReporter = new ActionReporter(Guid.NewGuid().ToString(), theLog, theExectuter);
		}

		[Test]
		public void circle_regex()
		{
			const string data = "<output class=\"mono\"/>\n\nName: Tayek   Race: Elf   Guild: Warrior Mage\nGender: Male   Age: 51   Circle: 8\nYou were born on the 3rd day of the 6th month of Arhat the Fire Lion in the year of the Golden Panther, 359 years after the victory of Lanival the Redeemer.\n\nYour birthday is more than 1 month away.\n\n     Strength :  15              Reflex :  13  \n      Agility :  12            Charisma :  12  \n   Discipline :  12              Wisdom :  15  \n Intelligence :  14             Stamina :  15  \n\nConcentration : 72    Max : 72\n\n       Favors : 0\n         TDPs : 29\n  Encumbrance : Light Burden\n\nWealth:\n  No Kronars.\n  7 silver and 6 bronze Lirums (760 copper Lirums).\n  No Dokoras.\n\nDebt:\n  No debt.\n\n<output class=\"\"/>\n>\n";
			const string pattern = "Circle:\\s+(\\d+)$";

			var match = Regex.Match(data, pattern, RegexOptions.Multiline);

			Assert.AreEqual("Circle: 8", match.Value);
		}

		[Test]
		public void replaces_matches_with_values()
		{
			const string data = "Circle: 8";
			const string pattern = "Circle:\\s+(\\d+)$";
			const string action = "var circle $1;put #var circle $1";

			var token = new ActionToken();
			token.When = pattern;
			token.Action = action;

			var context = new ActionContext();
			context.Match = data;
			context.Token = token;

			var scriptContext = new ScriptContext(Guid.NewGuid().ToString(), "Name", CancellationToken.None, theServices, null);
			scriptContext.DebugLevel = 5;

			context.ScriptContext = scriptContext;

			theReporter.OnNext(context);

			Assert.AreEqual("var circle 8;put #var circle 8", theExectuter.LastBlocks);
		}
	}

	public class StubIfBlockExecuter : IIfBlockExecuter
	{
		public string LastBlocks { get; set;}

		public bool Evaluate(string block, ScriptContext context)
		{
			throw new NotImplementedException();
		}

		public Task<CompletionEventArgs> ExecuteBlocks(string blocks, ScriptContext context)
		{
			LastBlocks = blocks;

			return null;
		}
	}
}

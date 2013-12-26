using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse;
using NUnit.Framework;
using Pathfinder.Core.Client.Scripting;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class GrammerTester
	{
		private IfBlocksParser theParser;

		[SetUp]
		public void SetUp()
		{
			theParser = new IfBlocksParser();
		}

		[Test]
		public void parses_if_else()
		{
			const string input = "Cast:\n\tif (\"%snapCast\" = \"OFF\") then\n\t{\n\t\twaitfor fully prepared\n\t} else {\n\t\tpause 7\n\t}\n\tput cast\n\tmatchre ManaCheck You strain\n\tmatchre ExpCheck snap\n\tmatchwait 4\n\tgoto ExpCheck\n\nExpCheck:\n\tpause 1";

			var blocks = theParser.BlockFor(input);

			Assert.AreEqual("(\"%snapCast\" = \"OFF\")", blocks.IfEval);
			Assert.AreEqual("{\n\t\twaitfor fully prepared\n\t}", blocks.IfBlock);

			Assert.IsNull(blocks.ElseIf);
			Assert.IsNull(blocks.ElseIfBlock);

			Assert.AreEqual("{\n\t\tpause 7\n\t}", blocks.ElseBlock);
		}

		[Test]
		public void parses_if_else_without_brackets()
		{
			const string input = "Cast:\n\tif (\"%snapCast\" = \"OFF\") then\n\t\twaitfor fully prepared\n\telse\n\t\tpause 7\n\tput cast\n\tmatchre ManaCheck You strain\n\tmatchre ExpCheck snap\n\tmatchwait 4\n\tgoto ExpCheck\n\nExpCheck:\n\tpause 1";

			var blocks = theParser.BlockFor(input);

			Assert.AreEqual("(\"%snapCast\" = \"OFF\")", blocks.IfEval);
			Assert.AreEqual("waitfor fully prepared", blocks.IfBlock);

			Assert.IsNull(blocks.ElseIf);
			Assert.IsNull(blocks.ElseIfBlock);

			Assert.AreEqual("pause 7", blocks.ElseBlock);
		}

		[Test]
		public void parses_if_elseif_else()
		{
			var input = "Cast:\n\tif (\"%snapCast\" = \"OFF\") then\n\t{\n\t\twaitfor fully prepared\n\t} else if (\"something1\" = \"another\") then {\n\t\tpause 1\n\t} else {\n\t\tpause 7\n\t}\n\tput cast\n\tmatchre ManaCheck You strain\n\tmatchre ExpCheck snap\n\tmatchwait 4\n\tgoto ExpCheck";

			var blocks = theParser.BlockFor(input);

			Assert.AreEqual("(\"%snapCast\" = \"OFF\")", blocks.IfEval);
			Assert.AreEqual("{\n\t\twaitfor fully prepared\n\t}", blocks.IfBlock);

			Assert.AreEqual("(\"something1\" = \"another\")", blocks.ElseIf);
			Assert.AreEqual("{\n\t\tpause 1\n\t}", blocks.ElseIfBlock);

			Assert.AreEqual("{\n\t\tpause 7\n\t}", blocks.ElseBlock);
		}

		[Test]
		public void parses_if_elseif_else_multiple_lines()
		{
			const string input = "Cast:\n\tif (\"%snapCast\" = \"OFF\") then\n\t{\n\t\twaitfor fully prepared\n\t\tline 1\n\t} else if (\"something1\" = \"another\") then {\n\t\tpause 1\n\t\tline 2\n\t} else {\n\t\tpause 7\n\t\tline 3\n\t}\n\tput cast\n\tmatchre ManaCheck You strain\n\tmatchre ExpCheck snap\n\tmatchwait 4\n\tgoto ExpCheck\n\nExpCheck:\n\tpause 1";

			var blocks = theParser.BlockFor(input);

			Assert.AreEqual("(\"%snapCast\" = \"OFF\")", blocks.IfEval);
			Assert.AreEqual("{\n\t\twaitfor fully prepared\n\t\tline 1\n\t}", blocks.IfBlock);

			Assert.AreEqual("(\"something1\" = \"another\")", blocks.ElseIf);
			Assert.AreEqual("{\n\t\tpause 1\n\t\tline 2\n\t}", blocks.ElseIfBlock);

			Assert.AreEqual("{\n\t\tpause 7\n\t\tline 3\n\t}", blocks.ElseBlock);
		}

		[Test]
		public void parses_nextline_goto_without_brackets()
		{
			const string input = "ExpCheck:\n\tpause 1\n\tif ($%magicToTrain.LearningRate >= %maxexp) then\ngoto END\n\tgoto ManaCheck\n\nManaCheck:";

			var blocks = theParser.BlockFor(input);

			Assert.AreEqual("($%magicToTrain.LearningRate >= %maxexp)", blocks.IfEval);
			Assert.AreEqual("goto END", blocks.IfBlock);
		}

		[Test]
		public void parses_goto_without_brackets()
		{
			const string input = "ExpCheck:\n\tpause 1\n\tif ($%magicToTrain.LearningRate >= %maxexp) then goto END\n\tgoto ManaCheck";

			var blocks = theParser.BlockFor(input);

			Assert.AreEqual("($%magicToTrain.LearningRate >= %maxexp)", blocks.IfEval);
			Assert.AreEqual("goto END", blocks.IfBlock);
		}

		[Test]
		public void parses_if_blocks()
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Pathfinder.Core.Client.Tests.Data.if_script.txt"))
			using (var reader = new StreamReader(stream)) {
				var data = reader.ReadToEnd();

				var parser = new IfBlocksParser();
				var blocks = parser.For(data).ToList();

				Assert.AreEqual(3, blocks.Count);

				var firstBlock = blocks.First();

				Assert.AreEqual(5, firstBlock.IfEvalLineNumber);
				Assert.AreEqual(6, firstBlock.IfBlockLineNumber);

				Assert.AreEqual(9, firstBlock.ElseIfLineNumber);
				Assert.AreEqual(9, firstBlock.ElseIfBlockLineNumber);

				Assert.AreEqual(12, firstBlock.ElseBlockLineNumber);
			}
		}
	}
}

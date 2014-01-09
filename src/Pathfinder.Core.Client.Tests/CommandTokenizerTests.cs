using System;
using System.Linq;
using NUnit.Framework;
using Outlander.Core.Client.Scripting;

namespace Outlander.Core.Client.Tests
{
	[TestFixture]
	public class CommandTokenizerTests
	{
		private Tokenizer theTokenizer;

		[SetUp]
		public void SetUp()
		{
			theTokenizer = new Tokenizer(TokenDefinitionRegistry.ClientCommands().Definitions());
		}

		[Test]
		public void creates_script_token_from_dot()
		{
			const string line = ".myscript one two three";

			var tokens = theTokenizer.Tokenize(line).ToList();
			var scriptToken = tokens[0] as ScriptToken;
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("script", scriptToken.Type);
			Assert.AreEqual("myscript", scriptToken.Name);
			Assert.True(new string[]{ "one", "two", "three" }.SequenceEqual(scriptToken.Args));
			Assert.AreEqual("myscript one two three", scriptToken.Value);
		}

		[Test]
		public void script_token_from_dot_handles_quoted_params()
		{
			const string line = ".myscript one two \"three four\"";

			var tokens = theTokenizer.Tokenize(line).ToList();
			var scriptToken = tokens[0] as ScriptToken;
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("script", tokens[0].Type);
			Assert.AreEqual("myscript one two \"three four\"", tokens[0].Value);
			Assert.AreEqual("myscript", scriptToken.Name);
			Assert.True(new string[]{ "one", "two", "three four" }.SequenceEqual(scriptToken.Args));
		}

		[Test]
		public void script_token_from_dot_handles_multiple_quoted_params()
		{
			const string line = ".myscript one two \"three four\" \"five six\"";

			var tokens = theTokenizer.Tokenize(line).ToList();
			var scriptToken = tokens[0] as ScriptToken;
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("script", tokens[0].Type);
			Assert.AreEqual("myscript one two \"three four\" \"five six\"", tokens[0].Value);
			Assert.AreEqual("myscript", scriptToken.Name);
			Assert.True(new string[]{ "one", "two", "three four", "five six" }.SequenceEqual(scriptToken.Args));
		}

		[Test]
		public void creates_script_abort_command_token()
		{
			const string line = "#script abort myscript";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("scriptcommand", tokens[0].Type);
			Assert.AreEqual("abort myscript", tokens[0].Value);
		}

		[Test]
		public void creates_script_pause_command_token()
		{
			const string line = "#script pause myscript";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("scriptcommand", tokens[0].Type);
			Assert.AreEqual("pause myscript", tokens[0].Value);
		}

		[Test]
		public void creates_parse_token()
		{
			const string line = "#parse something";

			var token = theTokenizer.Tokenize(line).Single();
			Assert.AreEqual("parse", token.Type);
			Assert.AreEqual("something", token.Value);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class TokenizerTests
	{
		private Tokenizer theTokenizer;

		[SetUp]
		public void SetUp()
		{
			theTokenizer = new Tokenizer(TokenDefinitionRegistry.Default().Definitions());
		}

		[Test]
		public void creates_label_token()
		{
			const string line = "somewhere:";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("label", tokens[0].Type);
			Assert.AreEqual("somewhere", tokens[0].Value);
		}

		[Test]
		public void creates_goto_token()
		{
			const string line = "goto somewhere";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("goto", tokens[0].Type);
			Assert.AreEqual("somewhere", tokens[0].Value);
		}

		[Test]
		public void creates_waitfor_token()
		{
			const string line = "waitfor You finish playing";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("waitfor", tokens[0].Type);
			Assert.AreEqual("You finish playing", tokens[0].Value);
		}

		[Test]
		public void creates_pause_token()
		{
			const string line = "pause 0.5";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("pause", tokens[0].Type);
			Assert.AreEqual("0.5", tokens[0].Value);
		}

		[Test]
		public void creates_pause_token_without_value()
		{
			const string line = "pause";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("pause", tokens[0].Type);
			Assert.AreEqual("", tokens[0].Value);
		}

		[Test]
		public void creates_put_token()
		{
			const string line = "PUT collect rocks";

			var tokens = theTokenizer.Tokenize(line).ToList();
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("put", tokens[0].Type);
			Assert.AreEqual("collect rocks", tokens[0].Value);
		}

		[Test]
		public void creates_match_token()
		{
			const string line = "match Kick You manage to collect";

			var token = theTokenizer.Tokenize(line).Single().As<MatchToken>();
			Assert.AreEqual("match", token.Type);
			Assert.AreEqual("Kick", token.Goto);
			Assert.AreEqual("You manage to collect", token.Pattern);
		}

		[Test]
		public void creates_match_token_2()
		{
			const string line = "match Wait1 ...wait something";

			var token = theTokenizer.Tokenize(line).Single().As<MatchToken>();
			Assert.AreEqual("match", token.Type);
			Assert.AreEqual("Wait1", token.Goto);
			Assert.AreEqual("...wait something", token.Pattern);
		}

		[Test]
		public void creates_matchre_token()
		{
			const string line = "matchre CheckEXP You take a step back|Now what did the|I could not find";
			const string pattern = "You take a step back|Now what did the|I could not find";

			var token = theTokenizer.Tokenize(line).Single().As<MatchToken>();
			Assert.AreEqual("matchre", token.Type);
			Assert.AreEqual("CheckEXP", token.Goto);
			Assert.AreEqual(pattern, token.Pattern);
		}

		[Test]
		public void creates_if_token()
		{
			const string line = "if ($Outdoorsmanship.LearningRate >= %maxexp) then goto END";

			var token = theTokenizer.Tokenize(line).Single().As<IfToken>();
			Assert.AreEqual("if", token.Type);
			Assert.AreEqual("($Outdoorsmanship.LearningRate >= %maxexp) then goto END", token.Value);
			//Assert.AreEqual("($Outdoorsmanship.LearningRate >= %maxexp)", token.If);
			//Assert.AreEqual("goto END", token.Then);
		}
	}
}

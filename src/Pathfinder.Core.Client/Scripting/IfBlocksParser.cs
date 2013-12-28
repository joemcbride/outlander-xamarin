using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Eto.Parse;
using Pathfinder.Core.Client.Scripting;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace Pathfinder.Core.Client
{
	public interface IIfBlocksParser
	{
		IEnumerable<IfBlocks> For(string script);
		int LineFromPos(string s, int pos);
		IfBlocks BlockFor(string snippet, int line = 0);
	}

	public class IfBlocksParser : IIfBlocksParser
	{
		private const string If_Blocks_Regex = "^[\\s]*(if)\\s";

		private readonly Grammar _grammar;

		public IfBlocksParser()
		{
			_grammar = BuildGrammar();
		}

		public IEnumerable<IfBlocks> For(string script)
		{
			var rmatches = Regex.Matches(script, If_Blocks_Regex, RegexOptions.Multiline);

			var matches = rmatches.Cast<RegexMatch>().Select(x => x.Index).ToList();

			var list = new List<IfBlocks>();

			for(int i = 0; i < matches.Count; i++)
			{
				var lineIdx = matches[i];
				var nextLineIdx = i + 1;
				var nextLineLength = nextLineIdx >= matches.Count
				                     ? script.Length - lineIdx
				                     : matches[nextLineIdx] - lineIdx;

				var value = script.Substring(lineIdx, nextLineLength);
				var block = BlockFor(value, LineFromPos(script, lineIdx));
				//block.IfEvalLineNumber = LineFromPos(script, lineIdx);
				list.Add(block);
			}

			return list;
		}

		public int LineFromPos(string s, int pos)
		{
			int res = 0;
			for (int i = 0; i <= pos; i++)
				if (s[i] == '\n') res++;
			return res;
		}

		public IfBlocks BlockFor(string snippet, int line = 0)
		{
			var match = _grammar.Match(snippet);

			var ifEvalMatch = match["ifBlock"]["if"];
			var ifEval = ifEvalMatch.Value;

			var ifBlockMatch = match["ifBlock"]["block"];
			var ifBlock = ifBlockMatch.Value;

			var elseIfMatch = match["elseif"]["if"];
			var elseIf = elseIfMatch.Value;

			var elseIfBlockMatch = match["elseif"]["block"];
			var elseIfBlock = elseIfBlockMatch.Value;

			var elseBlockMatch = match["else"]["block"];
			var elseBlock = elseBlockMatch.Value;

			var blocks = new IfBlocks();

			ifEval.IfNotNull(v => blocks.IfEval = v.ToString());
			blocks.IfEvalLineNumber = LineFromPos(snippet, ifEvalMatch.Index) + line;

			ifBlock.IfNotNull(v => blocks.IfBlock = v.ToString());
			blocks.IfBlockLineNumber = LineFromPos(snippet, ifBlockMatch.Index) + line;

			elseIf.IfNotNull(v => blocks.ElseIf = v.ToString());
			blocks.ElseIfLineNumber = LineFromPos(snippet, elseIfMatch.Index) + line;

			elseIfBlock.IfNotNull(v => blocks.ElseIfBlock = v.ToString());
			blocks.ElseIfBlockLineNumber = LineFromPos(snippet, elseIfBlockMatch.Index) + line;

			elseBlock.IfNotNull(v => blocks.ElseBlock = v.ToString());
			blocks.ElseBlockLineNumber = LineFromPos(snippet, elseBlockMatch.Index) + line;

			return blocks;
		}

		private Grammar BuildGrammar()
		{
			// optional repeating whitespace
			var ws = Terminals.WhiteSpace.Repeat(0);
			var ws1 = Terminals.WhiteSpace.Repeat(1);

			// parse a value with or without brackets
			var valueParser = Terminals.Set('(')
				.Then(Terminals.AnyChar.Repeat().Until(ws.Then(')')).Named("value"))
				.Then(Terminals.Set(')'))
				.SeparatedBy(ws)
				.Or(Terminals.WhiteSpace.Inverse().Repeat().Named("value"));

			var nextLine =
				Terminals.WhiteSpace.Inverse().Repeat().Until(Terminals.Eol)
					.Then(ws1)
					.Then(Terminals.AnyChar.Repeat().Until(Terminals.Eol)).Named("value");

			var bracketParser = Terminals.Set('{')
				.Then(Terminals.AnyChar.Repeat().Until(ws.Then('}')).Named("value"))
				.Then(Terminals.Set('}'))
				.SeparatedBy(ws)
				.Or(nextLine);
			//.Or(Terminals.AnyChar.Repeat().Until(Terminals.Eol));
			//.Or(Terminals.WhiteSpace.Inverse().Repeat(0).Named("value"));

			var ifParser = Terminals.Literal("if")
				.Then(ws1)
				.Then(valueParser.Named("if"))
				.Then(ws1)
				.Then(Terminals.Literal("then"))
				.Then(ws1)
				.Then(bracketParser.Named("block"));

			var elseIfParser = Terminals.Literal("else if")
				.Then(ws1)
				.Then(valueParser.Named("if"))
				.Then(ws1)
				.Then(Terminals.Literal("then"))
				.Then(ws1)
				.Then(bracketParser.Named("block"));

			var elseParser = Terminals.Literal("else")
				.Then(ws)
				.Then(bracketParser.Named("block"));

			return new Grammar(
				ws
				.Then(Terminals.AnyChar.Repeat(0).Until(ws.Then(Terminals.Literal("if"))))
				.Then(ifParser.Named("ifBlock"))
				.Then(ws)
				.Then(elseIfParser.Named("elseif").Optional())
				.Then(ws)
				.Then(elseParser.Named("else"))
				.Then(Terminals.End)
				.SeparatedBy(ws)
			);
		}
	}
}

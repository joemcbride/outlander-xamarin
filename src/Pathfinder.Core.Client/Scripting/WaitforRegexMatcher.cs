using System;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client.Scripting
{
	public class WaitforRegexMatcher : WaitForMatcher
	{
		public WaitforRegexMatcher(ScriptContext context, Token token)
			: base(context, token)
		{
		}

		public override MatchResult Checkmatch(string text)
		{
			var regexMatch = Regex.Match(text, Token.Value);

			return regexMatch.Success ? MatchResult.True() : MatchResult.False();
		}
	}
}

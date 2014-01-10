using System;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client.Scripting
{
	public class WaitforRegexMatcher : WaitForMatcher
	{
		public WaitforRegexMatcher(ScriptContext context, Token token)
			: base(context, token)
		{
		}

		public override MatchResult Checkmatch(string text)
		{
			var regexMatch = Regex.Match(text, Token.Value, RegexOptions.Multiline);

			return regexMatch.Success ? MatchResult.True() : MatchResult.False();
		}
	}
}

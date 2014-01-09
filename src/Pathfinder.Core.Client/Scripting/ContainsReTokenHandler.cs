using System;
using System.Linq;
using System.Text.RegularExpressions;
using Outlander.Core;
using Outlander.Core.Client;

namespace Outlander.Core.Client
{
	public class ContainsReTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var splitArgs = Regex
				.Matches(Token.Value, RegexPatterns.Arguments)
				.Cast<Match>()
				.Select(m => m.Groups["match"].Value)
				.ToArray();

			if(splitArgs.Length == 3)
			{
				Context.LocalVars.Set(splitArgs[0], Regex.IsMatch(splitArgs[1], splitArgs[2]).ToString());
			}

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

using System;

namespace Outlander.Core.Client.Scripting
{
	public class MatchTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var matchToken = (MatchToken)Token;

			if(!string.IsNullOrWhiteSpace(matchToken.Pattern)
				&& !string.IsNullOrWhiteSpace(matchToken.Goto)) {
				Context.MatchWait.Add(matchToken);
			}

			TaskSource.SetResult(new CompletionEventArgs());
		}
	}
}

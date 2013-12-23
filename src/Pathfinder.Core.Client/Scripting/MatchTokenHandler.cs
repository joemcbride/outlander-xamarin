using System;

namespace Pathfinder.Core.Client.Scripting
{
	public class MatchTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var matchToken = (MatchToken)Token;

			Context.Get<IScriptLog>().Log(Context.Name, "adding match for '" + matchToken.Pattern + "'", Context.LineNumber);

			Context.MatchWait.Add(matchToken);

			TaskSource.SetResult(new CompletionEventArgs());
		}
	}
}

using System;

namespace Pathfinder.Core.Client
{
	public class GotoTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			TaskSource.SetResult(new CompletionEventArgs { Goto = Token.Value });
		}
	}
}

using System;

namespace Pathfinder.Core.Client
{
	public class UnVarTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			Context.LocalVars.Remove(Token.Value);

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

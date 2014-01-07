using System;
using Pathfinder.Core.Client;

namespace Outlander.Core.Client
{
	public class DebugLevelTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			int level;

			if(Int32.TryParse(Token.Value, out level))
			{
				Context.DebugLevel = level;
			}

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

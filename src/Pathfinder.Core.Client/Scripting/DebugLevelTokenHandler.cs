using System;
using Outlander.Core.Client;
using Outlander.Core;

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

				if(Context.DebugLevel > 0) {
					Context.Get<IScriptLog>().Log(Context.Name, "debuglevel {0}".ToFormat(level), Context.LineNumber);
				}
			}

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

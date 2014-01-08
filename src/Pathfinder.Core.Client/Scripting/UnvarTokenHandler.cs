using System;

namespace Pathfinder.Core.Client
{
	public class UnVarTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			if(Context.DebugLevel > 0) {
				Context.Get<IScriptLog>().Log(Context.Name, "unvar " + Token.Value, Context.LineNumber);
			}

			Context.LocalVars.Remove(Token.Value);

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

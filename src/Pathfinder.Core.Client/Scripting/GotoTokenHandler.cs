using System;

namespace Pathfinder.Core.Client
{
	public class GotoTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var log = Context.Get<IScriptLog>();
			log.Log(Context.Name, "goto " + Token.Value, Context.LineNumber);

			TaskSource.SetResult(new CompletionEventArgs { Goto = Token.Value });
		}
	}
}

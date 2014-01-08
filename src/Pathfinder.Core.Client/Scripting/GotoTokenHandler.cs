using System;

namespace Pathfinder.Core.Client
{
	public class GotoTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var eval = Context.Get<ICommandProcessor>();
			var log = Context.Get<IScriptLog>();

			var location = eval.Eval(Token.Value, Context);

			if(Context.DebugLevel > 0) {
				log.Log(Context.Name, "goto " + location, Context.LineNumber);
			}

			TaskSource.SetResult(new CompletionEventArgs { Goto = location });
		}
	}
}

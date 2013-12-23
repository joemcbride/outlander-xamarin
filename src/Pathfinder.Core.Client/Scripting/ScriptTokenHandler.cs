using System;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client
{
	public class ScriptTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var runner = Context.Get<IScriptRunner>();
			runner.Run((ScriptToken)Token);

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

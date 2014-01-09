using System;
using Outlander.Core.Client.Scripting;

namespace Outlander.Core.Client
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

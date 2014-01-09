using System;

namespace Outlander.Core.Client
{
	public class SaveTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			if(Context.DebugLevel > 0) {
				Context.Get<IScriptLog>().Log(Context.Name, "saving {0}".ToFormat(Token.Value), Context.LineNumber);
			}
			Context.LocalVars.Set("s", Token.Value);

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

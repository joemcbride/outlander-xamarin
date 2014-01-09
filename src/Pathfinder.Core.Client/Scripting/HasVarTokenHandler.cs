using System;

namespace Outlander.Core.Client
{
	public class HasVarTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var log = Context.Get<IScriptLog>();

			if(Context.DebugLevel > 0) {
				log.Log(Context.Name, "hasvar " + Token.Value, Context.LineNumber);
			}

			var values = Token.Value.Split(new string[]{ " " }, StringSplitOptions.None);

			var hasVar = Context.LocalVars.HasKey(values[0]);

			if(Context.DebugLevel > 0) {
				log.Log(Context.Name, "setvar {0} {1}".ToFormat(values[1], hasVar), Context.LineNumber);
			}

			Context.LocalVars.Set(values[1], hasVar.ToString());

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

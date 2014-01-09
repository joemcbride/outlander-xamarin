using System;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client
{
	public class VarTokenHandler : TokenHandler
	{
		private const string Var_Regex = "^([\\w\\.]+)\\s([\\w%\\$].*)";

		protected override void execute()
		{
			var replacer = Context.Get<IVariableReplacer>();
			var log = Context.Get<IScriptLog>();

			var match = Regex.Match(Token.Value, Var_Regex);
			if(match.Success)
			{
				var value = replacer.Replace(match.Groups[2].Value, Context);
				if(Context.DebugLevel > 0) {
					log.Log(Context.Name, "setvar {0} {1}".ToFormat(match.Groups[1].Value, value), Context.LineNumber);
				}
				Context.LocalVars.Set(match.Groups[1].Value, value);
			}

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

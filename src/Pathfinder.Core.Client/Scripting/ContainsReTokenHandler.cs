using System;
using System.Linq;
using System.Text.RegularExpressions;
using Outlander.Core;
using Outlander.Core.Client;

namespace Outlander.Core.Client
{
	public class ContainsReTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var replacer = Context.Get<IVariableReplacer>();

			var splitArgs = Regex
				.Matches(Token.Value, RegexPatterns.Arguments)
				.Cast<Match>()
				.Select(m => m.Groups["match"].Value.Trim('"'))
				.ToArray();

			if(splitArgs.Length == 3)
			{
				var arg1 = replacer.Replace(splitArgs[1], Context);
				var arg2 = replacer.Replace(splitArgs[2], Context);
				var match = Regex.Match(arg1, arg2);
				if(Context.DebugLevel > 0) {
					Context.Get<IScriptLog>().Log(Context.Name, "containsre(\"{0}\", \"{1}\")".ToFormat(arg1, arg2), Context.LineNumber);
					Context.Get<IScriptLog>().Log(Context.Name, "setvar {0} {1}".ToFormat(splitArgs[0], match.Success), Context.LineNumber);
				}
				Context.LocalVars.Set(splitArgs[0], match.Success.ToString());
			}

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

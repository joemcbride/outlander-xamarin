using System;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client
{
	public class GlobalVarTokenHandler : TokenHandler
	{
		private const string Var_Regex = "^([\\w\\.]+)\\s([\\w%\\$].*)";

		protected override void execute()
		{
			var match = Regex.Match(Token.Value, Var_Regex);
			if(match.Success)
			{
				var replacer = Context.Get<IVariableReplacer>();
				var log = Context.Get<IScriptLog>();
				var gameState = Context.Get<IGameState>();
				var settingsLoader = Context.Get<AppSettingsLoader>();

				var value = replacer.Replace(match.Groups[2].Value, Context);
				if(Context.DebugLevel > 0) {
					log.Log(Context.Name, "setglobalvar {0} {1}".ToFormat(match.Groups[1].Value, value), Context.LineNumber);
				}
				gameState.Set(match.Groups[1].Value, value);

				settingsLoader.SaveVariables();
			}

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

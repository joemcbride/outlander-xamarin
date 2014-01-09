using System;
using System.Text.RegularExpressions;
using System.Threading;
using Outlander.Core.Client.Scripting;

namespace Outlander.Core.Client
{
	public class ScriptCommandTokenHandler : TokenHandler
	{
		private const string Command_Regex = "(\\w+)";

		protected override void execute()
		{
			var scriptRunner = Context.Get<IScriptRunner>();

			if(string.IsNullOrWhiteSpace(Token.Value))
				return;

			var commands = Token.Value.Split(new string[]{ " " }, StringSplitOptions.RemoveEmptyEntries);

			if(commands != null && commands.Length > 1)
			{
				var scriptToken = new ScriptToken { Name = commands[1] };

				var command = commands[0];

				if(command == "abort")
					scriptRunner.Stop(scriptToken);

				if(command == "pause")
					scriptRunner.Pause(scriptToken);

				if(command == "resume")
					scriptRunner.Resume(scriptToken);

				if(command == "vars")
					scriptRunner.Vars(scriptToken);
			}

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

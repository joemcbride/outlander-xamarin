using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client
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
			}
		}
	}
	
}

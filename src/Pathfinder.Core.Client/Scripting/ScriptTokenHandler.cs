using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client
{
	public class ScriptTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var runner = Context.Get<IScriptRunner>();
			runner.Run((ScriptToken)Token);
		}
	}
	
}

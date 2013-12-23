using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Client.Scripting;
using Pathfinder.Core.Tests;
using System.Threading.Tasks;

namespace Pathfinder.Core.Client.Tests
{
	public class StubScriptRunner : IScriptRunner
	{
		public ScriptToken RunToken { get; set; }
		public ScriptToken StopToken { get; set; }
		public ScriptToken PauseToken { get; set; }
		public ScriptToken ResumeToken { get; set; }

		public IEnumerable<IScript> Scripts()
		{
			throw new NotImplementedException();
		}

		public Task Run(ScriptToken token)
		{
			RunToken = token;

			return null;
		}

		public void Stop(ScriptToken token)
		{
			StopToken = token;
		}

		public void Pause(ScriptToken token)
		{
			PauseToken = token;
		}

		public void Resume(ScriptToken token)
		{
			ResumeToken = token;
		}
	}
}

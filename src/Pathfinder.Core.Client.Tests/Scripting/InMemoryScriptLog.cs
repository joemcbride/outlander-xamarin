using System;
using System.Text;

namespace Pathfinder.Core.Client.Tests
{
	public class InMemoryScriptLog : IScriptLog
	{
		public StringBuilder Builder = new StringBuilder();

		public event EventHandler<ScriptLogInfo> Info = delegate { };
		public event EventHandler<ScriptLogInfo> NotifyStarted = delegate { };
		public event EventHandler<ScriptLogInfo> NotifyAborted = delegate { };

		public void Started(string name, DateTime startTime)
		{
			Builder.AppendLine("{0} started".ToFormat(name));
		}

		public void Log(string name, string data, int lineNumber)
		{
			Builder.AppendLine(data);
		}

		public void Aborted(string name, DateTime startTime, TimeSpan runtime)
		{
			Builder.AppendLine("{0} aborted::{1}".ToFormat(name, runtime));
		}
	}
}

using System;

namespace Pathfinder.Core.Client
{
	public interface IScriptLog
	{
		event EventHandler<ScriptLogInfo> Info;
		event EventHandler<ScriptLogInfo> NotifyStarted;
		event EventHandler<ScriptLogInfo> NotifyAborted;
		void Log(string name, string data, int lineNumber);
		void Started(string name);
		void Aborted(string name, TimeSpan runtime);
	}

	public class ScriptLog : IScriptLog
	{
		public event EventHandler<ScriptLogInfo> Info = delegate { };
		public event EventHandler<ScriptLogInfo> NotifyStarted = delegate { };
		public event EventHandler<ScriptLogInfo> NotifyAborted = delegate { };

		public void Log(string name, string data, int lineNumber)
		{
			FireInfoLog(new ScriptLogInfo{ Name = name, Data = data, LineNumber = lineNumber });
		}

		public void Started(string name)
		{
			FireStartedLog(new ScriptLogInfo{ Name = name });
		}

		public void Aborted(string name, TimeSpan runtime)
		{
			FireAbortedLog(new ScriptLogInfo{ Name = name, Runtime = runtime });
		}

		private void FireInfoLog(ScriptLogInfo text)
		{
			var ev = Info;
			if(ev != null)
			{
				ev(this, text);
			}
		}

		private void FireStartedLog(ScriptLogInfo text)
		{
			var ev = NotifyStarted;
			if(ev != null)
			{
				ev(this, text);
			}
		}

		private void FireAbortedLog(ScriptLogInfo text)
		{
			var ev = NotifyAborted;
			if(ev != null)
			{
				ev(this, text);
			}
		}
	}

	public class ScriptLogInfo
	{
		public string Name { get; set; }
		public string Data { get; set; }
		public int LineNumber { get; set; }
		public TimeSpan Runtime { get; set; }
	}
}

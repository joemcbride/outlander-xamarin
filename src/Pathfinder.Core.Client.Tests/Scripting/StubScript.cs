using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Outlander.Core.Client.Scripting;
using Outlander.Core.Tests;

namespace Outlander.Core.Client.Tests
{
	public class StubScript : IScript
	{
		public bool Stopped { get; private set; }
		public string Script { get; set; }
		public string[] Args { get; set; }

		public TaskCompletionSource<object> CompletionSource { get; set; }

		public DateTime StartTime
		{
			get;
			private set;
		}

		public void Stop()
		{
			Stopped = true;
		}

		public Task Run(string id, string name, string script, params string[] args)
		{
			Id = id;
			Name = name;
			Script = script;
			Args = args;

			StartTime = DateTime.Now;

			CompletionSource = new TaskCompletionSource<object>();

			return CompletionSource.Task;
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public System.Collections.Generic.IDictionary<string, string> ScriptVars {
			get {
				throw new NotImplementedException();
			}
		}
	}
}

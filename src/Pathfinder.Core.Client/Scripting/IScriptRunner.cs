using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pathfinder.Core.Client.Scripting
{
	public interface IScriptRunner
	{
		IEnumerable<IScript> Scripts();

		Task Run(ScriptToken token);
		void Stop(ScriptToken token);
		void Pause(ScriptToken token);
		void Resume(ScriptToken token);
		void Vars(ScriptToken token);
	}

	public class ScriptRunner : IScriptRunner
	{
		private readonly IServiceLocator _services;
		private readonly IScriptLoader _scriptLoader;
		private readonly IScriptLog _scriptLog;
		private List<IScript> _scripts = new List<IScript>();

		private static Object LockObject = new object();

		public ScriptRunner(IServiceLocator services, IScriptLoader scriptLoader, IScriptLog scriptLog)
		{
			_services = services;
			_scriptLoader = scriptLoader;
			_scriptLog = scriptLog;

			DefaultCreate = () => new Script(_services, Tokenizer.With(TokenDefinitionRegistry.Default()));
			Create = DefaultCreate;
		}

		public Func<IScript> DefaultCreate { get; private set; }
		public Func<IScript> Create { get; set; }

		public IEnumerable<IScript> Scripts()
		{
			return _scripts.ToArray();
		}

		public Task Run(ScriptToken token)
		{
			RemoveByName(token.Name);

			if(!_scriptLoader.CanLoad(token.Name))
				return null;

			var data = _scriptLoader.Load(token.Name);

			var script = Create();
			_scripts.Add(script);

			return Task.Factory.StartNew(() => {
				var task = script.Run(token.Name, data, token.Args);

				task.ContinueWith(t => {
					RemoveByName(token.Name);
				});
			});
		}

		public void Stop(ScriptToken token)
		{
			RemoveByName(token.Name);
		}

		public void Pause(ScriptToken token)
		{
			FindByName(token.Name).IfNotNull(script => {
			});
		}

		public void Resume(ScriptToken token)
		{
			FindByName(token.Name).IfNotNull(script => {
			});
		}

		public void Vars(ScriptToken token)
		{
			FindByName(token.Name).IfNotNull(script => {

				var vars = script.ScriptVars.Select(p => "{0}: {1}".ToFormat(p.Key, p.Value));

				var runtime = DateTime.Now - script.StartTime;

				_scriptLog.Log(
					script.Name,
					"script vars\n{0}\ntotal runtime {1} seconds\n".ToFormat(
						string.Join("\n", vars),
						Math.Round(runtime.TotalSeconds, 2)),
					 -2);
			});
		}

		private IScript FindByName(string name)
		{
			lock (LockObject) {
				return _scripts.FirstOrDefault(x => x.Name == name);
			}
		}

		private void RemoveByName(string name)
		{
			lock (LockObject) {
				var script = _scripts.FirstOrDefault(x => x.Name == name);
				script.IfNotNull(s => {
					_scriptLog.Aborted(name, s.StartTime, DateTime.Now - s.StartTime);
					s.Stop();
					_scripts.Remove(s);
				});
			}
		}
	}
}

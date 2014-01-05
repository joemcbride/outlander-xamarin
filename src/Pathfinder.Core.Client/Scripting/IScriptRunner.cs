using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

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

		private static ReaderWriterLockSlim LockObject = new ReaderWriterLockSlim();

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
				var task = script.Run(token.Id, token.Name, data, token.Args);

				task.ContinueWith(t => {
					RemoveById(token.Id);
				});
			});
		}

		public void Stop(ScriptToken token)
		{
			if(!string.IsNullOrWhiteSpace(token.Id))
				RemoveById(token.Id);
			else
				RemoveByName(token.Name);
		}

		public void Pause(ScriptToken token)
		{
			FindById(token.Id).IfNotNull(script => {
			});
		}

		public void Resume(ScriptToken token)
		{
			FindById(token.Id).IfNotNull(script => {
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
			return LockObject.Read(() => {
				return _scripts.FirstOrDefault(x => x.Name == name);
			});
		}

		private IScript FindById(string id)
		{
			return LockObject.Read(() => {
				return _scripts.FirstOrDefault(x => x.Id == id);
			});
		}

		private void RemoveByName(string name)
		{
			LockObject.Write(() => {
				var script = _scripts.FirstOrDefault(x => x.Name == name);
				script.IfNotNull(s => {
					_scriptLog.Aborted(s.Name, s.StartTime, DateTime.Now - s.StartTime);
					s.Stop();
					_scripts.Remove(s);
				});
			});
		}

		private void RemoveById(string id)
		{
			LockObject.Write(() => {
				var script = _scripts.FirstOrDefault(x => x.Id == id);
				script.IfNotNull(s => {
					_scriptLog.Aborted(s.Name, s.StartTime, DateTime.Now - s.StartTime);
					s.Stop();
					_scripts.Remove(s);
				});
			});
		}
	}
}

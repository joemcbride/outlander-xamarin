using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Outlander.Core.Client;

namespace Pathfinder.Core.Client.Scripting
{
	public interface IScript
	{
		string Id { get; }
		string Name { get; }
		DateTime StartTime { get; }
		IDictionary<string, string> ScriptVars { get; }

		void Stop();
		Task Run(string id, string name, string script, params string[] args);
	}

	public class Script : IScript
	{
		private readonly IServiceLocator _serviceLocator;
		private readonly Tokenizer _tokenizer;
		private readonly IScriptLog _log;
		private readonly IIfBlocksParser _ifBlocksParser;

		private TaskCompletionSource<object> _taskCompletionSource;
		private CancellationTokenSource _taskCancelationSource;
		private ScriptContext _scriptContext;

		private string[] _scriptLines;
		private IDictionary<int, IfBlocks> _ifBlocks = new Dictionary<int, IfBlocks>();
		private IDictionary<string, int> _gotos = new Dictionary<string, int>();
		private ISimpleDictionary<string, string> _localVars = new SimpleDictionary<string, string>();
		private IDictionary<string, ITokenHandler> _tokenHandlers = new Dictionary<string, ITokenHandler>();

		private ActionTracker _actionTracker = new ActionTracker();
		private ActionReporter _actionReporter;

		public Script(IServiceLocator serviceLocator, Tokenizer tokenizer)
		{
			_serviceLocator = serviceLocator;
			_tokenizer = tokenizer;
			_taskCompletionSource = new TaskCompletionSource<object>();
			_taskCancelationSource = new CancellationTokenSource();

			_actionReporter = new ActionReporter("id", _serviceLocator.Get<IScriptLog>(), _serviceLocator.Get<IIfBlockExecuter>());
			_actionReporter.Subscribe(_actionTracker);

			_ifBlocksParser = _serviceLocator.Get<IIfBlocksParser>();
			_log = _serviceLocator.Get<IScriptLog>();

			_tokenHandlers["exit"] = new ExitTokenHandler();
			_tokenHandlers["comment"] = new ContinueTokenHandler();
			_tokenHandlers["debuglevel"] = new DebugLevelTokenHandler();
			_tokenHandlers["var"] = new VarTokenHandler();
			_tokenHandlers["globalvar"] = new GlobalVarTokenHandler();
			_tokenHandlers["unvar"] = new UnVarTokenHandler();
			_tokenHandlers["hasvar"] = new HasVarTokenHandler();
			_tokenHandlers["goto"] = new GotoTokenHandler();
			_tokenHandlers["waitfor"] = serviceLocator.Get<WaitForTokenHandler>();
			_tokenHandlers["waitforre"] = serviceLocator.Get<WaitForReTokenHandler>();
			_tokenHandlers["pause"] = new PauseTokenHandler();
			_tokenHandlers["put"] = new SendCommandTokenHandler();
			_tokenHandlers["echo"] = new EchoTokenHandler();
			_tokenHandlers["match"] = new MatchTokenHandler();
			_tokenHandlers["matchre"] = new MatchTokenHandler();
			_tokenHandlers["matchwait"] = serviceLocator.Get<MatchWaitTokenHandler>();
			_tokenHandlers["if"] = new IfTokenHandler();
			_tokenHandlers["if_"] = new IfArgTokenHandler();
			_tokenHandlers["save"] = new SaveTokenHandler();
			_tokenHandlers["move"] = new MoveTokenHandler();
			_tokenHandlers["nextroom"] = new NextroomTokenHandler();
			_tokenHandlers["send"] = new SendTokenHandler();
			_tokenHandlers["label"] = new ContinueTokenHandler((context, token) => {
				_log.Log(Name, "passing label: {0}".ToFormat(token.Value), context.LineNumber);
			});
		}

		public string Id { get; private set; }
		public string Name { get; private set; }
		public DateTime StartTime { get; private set; }

		public IDictionary<string, string> ScriptVars
		{
			get {
				return _localVars.Values();
			}
		}

		public void Stop()
		{
			_taskCancelationSource.Cancel();
		}

		public Task Run(string id, string name, string script, params string[] args)
		{
			StartTime = DateTime.Now;
			Id = id;
			Name = name;

			_log.Started(name, StartTime);

			_ifBlocksParser.For(script).Apply(x => {
				_ifBlocks[x.IfEvalLineNumber] = x;
			});

			//var start = DateTime.Now;
			//_log.Log(name, "after if: {0}".ToFormat(start), 0);

			_localVars["0"] = string.Join(" ", args);
			args.Apply((x, idx) => _localVars["{0}".ToFormat(idx + 1)] = x);

			_scriptContext = new ScriptContext(Id, Name, _taskCancelationSource.Token, _serviceLocator, _localVars);

			_scriptLines = script.Split(new string[]{ "\n" }, StringSplitOptions.None);

			//_log.Log(name, "after split: {0}".ToFormat(DateTime.Now - start), 0);

			_scriptLines.Apply((line, num) => {
				if(string.IsNullOrWhiteSpace(line)) return;
				var match = Regex.Match(line.TrimStart(), RegexPatterns.Label);
				if(match.Success) {
					_gotos[match.Groups[1].Value] = num;
				}
			});

			//_log.Log(name, "after gotos: {0}".ToFormat(DateTime.Now), 0);

			try
			{
				Execute(_taskCancelationSource.Token);
			}
			catch(OperationCanceledException)
			{
				_taskCompletionSource.TrySetCanceled();
				_actionTracker.EndTransmission();
			}
			catch(Exception exc)
			{
				_taskCompletionSource.TrySetException(exc);
				_log.Log(_scriptContext.Name, "Error: " + exc.Message, _scriptContext.LineNumber);
				_actionTracker.EndTransmission();
			}

			return _taskCompletionSource.Task;
		}

		private void Execute(CancellationToken cancelToken)
		{
			bool canceled = false;
			for(int i = 0; i < _scriptLines.Length; i++) {

				_scriptContext.LineNumber = i;

				cancelToken.ThrowIfCancellationRequested();

				var line = _scriptLines[i];
				var token = _tokenizer.Tokenize(line.TrimStart()).FirstOrDefault();
				if(token != null && !token.Ignore)
				{
					var actionToken = token as ActionToken;
					if(actionToken != null)
					{
						var actionContext = new ActionContext();
						actionContext.ScriptName = _scriptContext.Name;
						actionContext.LineNumber = i;
						actionContext.Token = actionToken;
						RunAction(actionContext, _actionTracker, _scriptContext.LocalVars);
						continue;
					}

					var ifToken = token as IfToken;
					if(ifToken != null && ifToken.ReplaceBlocks)
					{
						ifToken.Blocks = _ifBlocks[i];
					}

					var task = _tokenHandlers[token.Type].Execute(_scriptContext, token);
					try
					{
						task.Wait(cancelToken);
					}
					catch(AggregateException)
					{
						canceled = true;
						_taskCompletionSource.TrySetCanceled();
						_actionTracker.EndTransmission();
						break;
					}
					if(!string.IsNullOrWhiteSpace(task.Result.Goto))
					{
						if(!_gotos.ContainsKey(task.Result.Goto))
						{
							_log.Log(_scriptContext.Name, "cannot find label '{0}'".ToFormat(task.Result.Goto), i);
							canceled = true;
							_taskCompletionSource.TrySetCanceled();
							_actionTracker.EndTransmission();
							break;
						}

						i = _gotos[task.Result.Goto] - 1;
					}
					else {
						i = _scriptContext.LineNumber;
					}
				}
			}

			if(!canceled)
				_taskCompletionSource.TrySetResult(null);
		}

		private CancellationToken RunAction(ActionContext actionContext, IDataTracker<ActionContext> tracker,  ISimpleDictionary<string, string> localVars)
		{
			var cancelSource = new CancellationTokenSource();

			var context = new ScriptContext(
				_scriptContext.Id,
				_scriptContext.Name,
				cancelSource.Token,
				_serviceLocator,
				localVars);

			actionContext.ScriptContext = context;

			_scriptContext.CancelToken.Register(() => {
				cancelSource.Cancel();
			});

			Task.Factory.StartNew(() => {
				var handler = new ActionTokenHandler(actionContext, tracker);
				handler.Execute(context, actionContext.Token);
			}, cancelSource.Token);

			return cancelSource.Token;
		}
	}

	public class ActionTracker : DataTracker<ActionContext>
	{
	}

	public class ActionReporter : DataReporter<ActionContext>
	{
		private readonly IScriptLog _scriptLog;
		private readonly IIfBlockExecuter _executer;

		public ActionReporter(string id, IScriptLog scriptLog, IIfBlockExecuter executer)
			: base(id)
		{
			_scriptLog = scriptLog;
			_executer = executer;
		}

		public override void OnNext(ActionContext value)
		{
			_scriptLog.Log(value.ScriptName, "action triggered: {0}".ToFormat(value.Token.When), value.LineNumber);

			var result = Regex.Replace(value.Match, value.Token.When, value.Token.Action);

			_executer.ExecuteBlocks(result, value.ScriptContext);
		}
	}	
}

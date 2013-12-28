using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

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
		private ISimpleDictionary<int, IfBlocks> _ifBlocks = new SimpleDictionary<int, IfBlocks>();
		private ISimpleDictionary<string, int> _gotos = new SimpleDictionary<string, int>();
		private ISimpleDictionary<string, string> _localVars = new SimpleDictionary<string, string>();
		private ISimpleDictionary<string, ITokenHandler> _tokenHandlers = new SimpleDictionary<string, ITokenHandler>();

		public Script(IServiceLocator serviceLocator, Tokenizer tokenizer)
		{
			_serviceLocator = serviceLocator;
			_tokenizer = tokenizer;
			_taskCompletionSource = new TaskCompletionSource<object>();
			_taskCancelationSource = new CancellationTokenSource();

			_ifBlocksParser = _serviceLocator.Get<IIfBlocksParser>();
			_log = _serviceLocator.Get<IScriptLog>();

			_tokenHandlers["label"] = new ContinueTokenHandler((context, token) => {
				_log.Log(Name, "passing label: {0}".ToFormat(token.Value), context.LineNumber);
			});
			_tokenHandlers["comment"] = new ContinueTokenHandler();
			_tokenHandlers["var"] = new VarTokenHandler();
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
			_tokenHandlers["save"] = new SaveTokenHandler();
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

			_localVars["0"] = string.Join(" ", args);
			args.Apply((x, idx) => _localVars["{0}".ToFormat(idx + 1)] = x);

			_scriptContext = new ScriptContext(Id, Name, _taskCancelationSource.Token, _serviceLocator, _localVars);

			_scriptLines = script
				.Split(new string[]{ "\n", "\r" }, StringSplitOptions.None)
				.Select(s => s.TrimStart())
				.ToArray();

			_scriptLines.Apply((line, num) => {
				if(string.IsNullOrWhiteSpace(line)) return;
				var match = Regex.Match(line, RegexPatterns.Label);
				if(match.Success) {
					_gotos[match.Groups[1].Value] = num;
				}
			});

			try
			{
				Execute(_taskCancelationSource.Token);
			}
			catch(OperationCanceledException)
			{
				_taskCompletionSource.TrySetCanceled();
			}
			catch(Exception exc)
			{
				_taskCompletionSource.TrySetException(exc);
				_log.Log(_scriptContext.Name, "Error: " + exc.Message, _scriptContext.LineNumber);
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
				var token = _tokenizer.Tokenize(line).FirstOrDefault();
				if(token != null && !token.Ignore) {
					var ifToken = token as IfToken;
					if(ifToken != null)
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
						break;
					}
					if(!string.IsNullOrWhiteSpace(task.Result.Goto))
					{
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
	}
}

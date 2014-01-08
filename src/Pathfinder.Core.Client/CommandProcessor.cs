using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Outlander.Core.Client;

namespace Pathfinder.Core.Client
{
	public interface ICommandProcessor
	{
		string Eval(string command, ScriptContext context = null);
		Task Process(string command, ScriptContext context = null, bool echo = true);
		Task Echo(string command, ScriptContext context = null);
		Task Parse(string command, ScriptContext context = null);
	}

	public class CommandProcessor : ICommandProcessor
	{
		private readonly IServiceLocator _services;
		private readonly IVariableReplacer _variableReplacer;
		private readonly IScriptLog _scriptLog;
		private readonly Tokenizer _tokenizer;
		private readonly ISimpleDictionary<string, ITokenHandler> _tokenHandlers;

		public CommandProcessor(IServiceLocator services, IVariableReplacer variableReplacer, IScriptLog scriptLog)
		{
			_services = services;
			_variableReplacer = variableReplacer;
			_scriptLog = scriptLog;
			_tokenizer = Tokenizer.With(TokenDefinitionRegistry.ClientCommands());

			_tokenHandlers = new SimpleDictionary<string, ITokenHandler>();
			_tokenHandlers["script"] = new ScriptTokenHandler();
			_tokenHandlers["scriptcommand"] = new ScriptCommandTokenHandler();
			_tokenHandlers["send"] = new SendTokenHandler();
			_tokenHandlers["globalvar"] = new GlobalVarTokenHandler();
			_tokenHandlers["parse"] = new ParseTokenHandler();
		}

		public string Eval(string command, ScriptContext context = null)
		{
			if(context == null)
				context = new ScriptContext(Guid.NewGuid().ToString(), null, CancellationToken.None, _services, null);

			return _variableReplacer.Replace(command, context);
		}

		public Task Process(string command, ScriptContext context = null, bool echo = true)
		{
			var completionSource = new TaskCompletionSource<object>();

			var commands = command.Split(new string[]{ ";" }, StringSplitOptions.RemoveEmptyEntries);

			commands.Apply(c => {
				var task = ProcessInternal(c, context, echo);
				task.Wait();
			});

			completionSource.TrySetResult(null);

			return completionSource.Task;
		}

		private Task ProcessInternal(string command, ScriptContext context = null, bool echo = true)
		{
			var completionSource = new TaskCompletionSource<object>();

			var token = _tokenizer.Tokenize(command).FirstOrDefault();
			if(token != null) {
				if(context == null)
					context = new ScriptContext(Guid.NewGuid().ToString(), token.Type, CancellationToken.None, _services, null);

				var handler = _tokenHandlers[token.Type];
				Task.Factory.StartNew(() => {
					handler.Execute(context, token);
					completionSource.TrySetResult(null);
				});
			}
			else {
				var server = _services.Get<IGameServer>();
				var replaced = Eval(command, context);

				if(context != null && context.DebugLevel > 0)
					_scriptLog.Log(context.Name, "{0}".ToFormat(replaced), context.LineNumber);

				if(echo)
					EchoCommand(replaced + "\n", context);

				server.SendCommand(replaced);
				completionSource.TrySetResult(null);
			}

			return completionSource.Task;
		}

		public Task EchoCommand(string command, ScriptContext context = null)
		{
			var formatted = command;
			if(context != null && !string.IsNullOrWhiteSpace(context.Name)) {
				formatted = "[{0}] {1}".ToFormat(context.Name, command);
			}

			return Publish(formatted, context, t => {
				t.Color = "ADFF2F";
			});
		}

		public Task Echo(string command, ScriptContext context = null)
		{
			return Publish(command, context, t => {
				t.Color = "#00FFFF";
				t.Mono = true;
				if(context != null && context.DebugLevel > 0)
					_scriptLog.Log(context.Name, "echo {0}".ToFormat(t.Text), context.LineNumber);
			});
		}

		public Task Parse(string command, ScriptContext context = null)
		{
			return Publish(command, context, t => {
				t.Filtered = true;
				if(context != null && context.DebugLevel > 0)
					_scriptLog.Log(context.Name, "parse {0}".ToFormat(t.Text), context.LineNumber);
			});
		}

		private Task Publish(string command, ScriptContext context = null, Action<TextTag> configure = null)
		{
			var completionSource = new TaskCompletionSource<object>();

			var gameStream = _services.Get<IGameStream>();
			var replaced = Eval(command, context);

			var tag = TextTag.For(replaced);

			if(configure != null)
				configure(tag);

			gameStream.Publish(tag);
			completionSource.TrySetResult(null);

			return completionSource.Task;
		}
	}
}

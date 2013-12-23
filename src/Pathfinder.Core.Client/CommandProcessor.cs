using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client
{
	public interface ICommandProcessor
	{
		string Eval(string command, ScriptContext context = null);
		void Process(string command, ScriptContext context = null);
	}

	public class CommandProcessor : ICommandProcessor
	{
		private readonly IServiceLocator _services;
		private readonly IVariableReplacer _variableReplacer;
		private readonly IScriptLog _scriptLog;
		private readonly Tokenizer _tokenizer;
		private ISimpleDictionary<string, ITokenHandler> _tokenHandlers = new SimpleDictionary<string, ITokenHandler>();

		public CommandProcessor(IServiceLocator services, IVariableReplacer variableReplacer, IScriptLog scriptLog)
		{
			_services = services;
			_variableReplacer = variableReplacer;
			_scriptLog = scriptLog;
			_tokenizer = Tokenizer.With(TokenDefinitionRegistry.ClientCommands());
			_tokenHandlers["script"] = new ScriptTokenHandler();
			_tokenHandlers["scriptcommand"] = new ScriptCommandTokenHandler();
		}

		public string Eval(string command, ScriptContext context = null)
		{
			if(context == null)
				context = new ScriptContext(null, CancellationToken.None, _services, null);

			return _variableReplacer.Replace(command, context);
		}

		public void Process(string command, ScriptContext context = null)
		{
			var token = _tokenizer.Tokenize(command).FirstOrDefault();
			if(token != null) {
				if(context == null)
					context = new ScriptContext(token.Type, CancellationToken.None, _services, null);

				_tokenHandlers[token.Type].Execute(context, token);
			}
			else {
				var server = _services.Get<IGameServer>();
				var replaced = Eval(command, context);

				if(context != null)
					_scriptLog.Log(context.Name, "sending command: {0}".ToFormat(replaced), context.LineNumber);

				server.SendCommand(replaced);
			}
		}
	}
}

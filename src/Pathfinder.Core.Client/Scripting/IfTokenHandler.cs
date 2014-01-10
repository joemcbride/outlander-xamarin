using System;
using System.Linq;
using System.Threading.Tasks;
using DynamicExpresso;
using Outlander.Core.Client.Scripting;
using Outlander.Core.Client;
using Outlander.Core;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client
{
	public class IfTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var ifToken = Token as IfToken;

			var executer = Context.Get<IIfBlockExecuter>();

			bool setComplete = true;

			var completeResult = new CompletionEventArgs();

			try
			{
				var executeBlock = string.Empty;
				if(executer.Evaluate(ifToken.Blocks.IfEval, Context))
				{
					executeBlock = ifToken.Blocks.IfBlock;
					Context.LineNumber = ifToken.Blocks.IfBlockLineNumber;
				}
				else if (!string.IsNullOrWhiteSpace(ifToken.Blocks.ElseIf)
					&& executer.Evaluate(ifToken.Blocks.ElseIf, Context))
				{
					executeBlock = ifToken.Blocks.ElseIfBlock;
					Context.LineNumber = ifToken.Blocks.ElseIfBlockLineNumber;
				}
				else if(!string.IsNullOrWhiteSpace(ifToken.Blocks.ElseBlock))
				{
					executeBlock = ifToken.Blocks.ElseBlock;
					Context.LineNumber = ifToken.Blocks.ElseBlockLineNumber;
				}

				var task = executer.ExecuteBlocks(executeBlock, Context);
				task.Wait();
				if(string.IsNullOrWhiteSpace(task.Result.Goto))
				{
					// jump to end if if/else statement
					Context.LineNumber = ifToken.Blocks.LastLineNumber;
				}
				completeResult = task.Result;
			}
			catch(AggregateException)
			{
				setComplete = false;
				TaskSource.TrySetCanceled();
			}
			catch(Exception exc)
			{
				setComplete = false;
				TaskSource.TrySetException(exc);
			}

			if(setComplete)
				TaskSource.TrySetResult(completeResult);
		}
	}

	public interface IIfBlockExecuter
	{
		bool Evaluate(string block, ScriptContext context);
		Task<CompletionEventArgs> ExecuteBlocks(string blocks, ScriptContext context);
	}

	public class IfBlockExecuter : IIfBlockExecuter
	{
		private readonly Tokenizer _tokenizer;
		private readonly ISimpleDictionary<string, ITokenHandler> _tokenHandlers;

		public IfBlockExecuter(WaitForTokenHandler waitForTokenHandler, WaitForReTokenHandler waitForReTokenHandler, MatchWaitTokenHandler matchWaitTokenHandler)
		{
			_tokenizer = new Tokenizer(TokenDefinitionRegistry.Default().Definitions());
			_tokenHandlers = new SimpleDictionary<string, ITokenHandler>();

			_tokenHandlers["exit"] = new ExitTokenHandler();
			_tokenHandlers["comment"] = new ContinueTokenHandler();
			_tokenHandlers["debuglevel"] = new DebugLevelTokenHandler();
			_tokenHandlers["var"] = new VarTokenHandler();
			_tokenHandlers["unvar"] = new UnVarTokenHandler();
			_tokenHandlers["hasvar"] = new HasVarTokenHandler();
			_tokenHandlers["goto"] = new GotoTokenHandler();
			_tokenHandlers["waitfor"] = waitForTokenHandler;
			_tokenHandlers["waitforre"] = waitForReTokenHandler;
			_tokenHandlers["pause"] = new PauseTokenHandler();
			_tokenHandlers["put"] = new SendCommandTokenHandler();
			_tokenHandlers["echo"] = new EchoTokenHandler();
			_tokenHandlers["match"] = new MatchTokenHandler();
			_tokenHandlers["matchre"] = new MatchTokenHandler();
			_tokenHandlers["matchwait"] = matchWaitTokenHandler;
			_tokenHandlers["save"] = new SaveTokenHandler();
			_tokenHandlers["move"] = new MoveTokenHandler();
			_tokenHandlers["nextroom"] = new NextroomTokenHandler();
			_tokenHandlers["send"] = new SendTokenHandler();
			_tokenHandlers["parse"] = new ParseTokenHandler();
			_tokenHandlers["containsre"] = new ContainsReTokenHandler();
			_tokenHandlers["gosub"] = new GoSubTokenHandler();
			_tokenHandlers["return"] = new ReturnTokenHandler();
		}

		public bool Evaluate(string block, ScriptContext context)
		{
			var replacer = context.Get<IVariableReplacer>();
			var scriptLog = context.Get<IScriptLog>();

			var replaced = replacer.Replace(block, context);
			// replace single equals with double equals
			replaced = Regex.Replace(replaced, "([^=:])=(?!=)", "$1==");

			if(context.DebugLevel > 0) {
				scriptLog.Log(context.Name, "if {0}".ToFormat(replaced), context.LineNumber);
			}

			try
			{
				var interpreter = new Interpreter();
				var result = (bool)interpreter.Eval(replaced);
				if(context.DebugLevel > 0) {
					scriptLog.Log(context.Name, "if result {0}".ToFormat(result.ToString().ToLower()), context.LineNumber);
				}
				return result;
			}
			catch(Exception exc)
			{
				scriptLog.Log(context.Name, "if result {0}".ToFormat(exc), context.LineNumber);

				return false;
			}
		}

		public Task<CompletionEventArgs> ExecuteBlocks(string blocks, ScriptContext context)
		{
			var taskSource = new TaskCompletionSource<CompletionEventArgs>();

			var lines = blocks.Split(new string[]{ "\n", ";" }, StringSplitOptions.None);

			var setComplete = true;

			for(int i = 0; i < lines.Length; i++)
			{
				var line = lines[i];

				line.IfNotNull(l=>line = l.Trim());

				context.LineNumber++;

				if(string.IsNullOrWhiteSpace(line))
					continue;

				var token = _tokenizer.Tokenize(line).FirstOrDefault();
				if(token != null && !token.Ignore)
				{
					var task = _tokenHandlers[token.Type].Execute(context, token);
					try
					{
						task.Wait();
					}
					catch(AggregateException)
					{
						setComplete = false;
						taskSource.TrySetCanceled();
						break;
					}
					if(!string.IsNullOrWhiteSpace(task.Result.Goto))
					{
						setComplete = false;
						taskSource.TrySetResult(task.Result);
						break;
					}
				}
			}

			if(setComplete) {

				taskSource.TrySetResult(new CompletionEventArgs());
			}

			return taskSource.Task;
		}
	}
}

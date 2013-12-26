using System;
using System.Linq;
using System.Threading.Tasks;
using DynamicExpresso;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client
{
	public class IfTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var ifToken = Token as IfToken;

			var something = new Something();

			bool setComplete = true;

			var completeResult = new CompletionEventArgs();

			try
			{
				var executeBlock = string.Empty;
				if(something.Evaluate(ifToken.Blocks.IfEval, Context))
				{
					executeBlock = ifToken.Blocks.IfBlock;
					Context.LineNumber = ifToken.Blocks.IfBlockLineNumber;
				}
				else if (!string.IsNullOrWhiteSpace(ifToken.Blocks.ElseIf)
					&& something.Evaluate(ifToken.Blocks.ElseIf, Context))
				{
					executeBlock = ifToken.Blocks.ElseIfBlock;
					Context.LineNumber = ifToken.Blocks.ElseIfBlockLineNumber;
				}
				else if(!string.IsNullOrWhiteSpace(ifToken.Blocks.ElseBlock))
				{
					executeBlock = ifToken.Blocks.ElseBlock;
					Context.LineNumber = ifToken.Blocks.ElseBlockLineNumber;
				}

				var task = something.ExecuteBlocks(executeBlock, Context);
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

	public class Something
	{
		private readonly Tokenizer _tokenizer;
		private readonly ISimpleDictionary<string, TokenHandler> _tokenHandlers;

		public Something()
		{
			_tokenizer = new Tokenizer(TokenDefinitionRegistry.Default().Definitions());
			_tokenHandlers = new SimpleDictionary<string, TokenHandler>();

			_tokenHandlers["comment"] = new ContinueTokenHandler();
			_tokenHandlers["var"] = new VarTokenHandler();
			_tokenHandlers["goto"] = new GotoTokenHandler();
			_tokenHandlers["waitfor"] = new WaitForTokenHandler();
			_tokenHandlers["pause"] = new PauseTokenHandler();
			_tokenHandlers["put"] = new SendCommandTokenHandler();
			_tokenHandlers["echo"] = new EchoTokenHandler();
			_tokenHandlers["match"] = new MatchTokenHandler();
			_tokenHandlers["matchre"] = new MatchTokenHandler();
			_tokenHandlers["matchwait"] = new MatchWaitTokenHandler();
		}

		public bool Evaluate(string block, ScriptContext context)
		{
			var replacer = context.Get<IVariableReplacer>();
			var scriptLog = context.Get<IScriptLog>();

			var replaced = replacer.Replace(block, context);

			scriptLog.Log(context.Name, "if {0}".ToFormat(replaced), context.LineNumber);

			try
			{
				var interpreter = new Interpreter();
				var result = (bool)interpreter.Eval(replaced);
				scriptLog.Log(context.Name, "if result {0}".ToFormat(result.ToString().ToLower()), context.LineNumber);
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

			var lines = blocks.Split(new string[]{ "\n" }, StringSplitOptions.None);

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

using System;

namespace Outlander.Core.Client
{
	public class IfArgTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var ifToken = Token as IfToken;

			var executer = Context.Get<IIfBlockExecuter>();

			bool setComplete = true;

			var completeResult = new CompletionEventArgs();
			var hasKey = Context.LocalVars.HasKey(ifToken.Blocks.IfEval);

			if(Context.DebugLevel > 0) {
				Context.Get<IScriptLog>().Log(Context.Name, ifToken.Value, Context.LineNumber);
				Context.Get<IScriptLog>().Log(Context.Name, "if result {0}".ToFormat(hasKey), Context.LineNumber);
			}

			if(hasKey)
			{
				try
				{
					var task = executer.ExecuteBlocks(ifToken.Blocks.IfBlock, Context);
					task.Wait();
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
			}

			if(setComplete)
				TaskSource.TrySetResult(completeResult);
		}
	}
}

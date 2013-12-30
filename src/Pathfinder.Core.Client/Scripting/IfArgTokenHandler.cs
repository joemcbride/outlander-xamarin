using System;

namespace Pathfinder.Core.Client
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

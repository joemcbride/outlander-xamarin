using System;

namespace Pathfinder.Core.Client
{
	public class SendTokenHandler : TokenHandler
	{
		private ISendQueue _sendQueue;

		protected override void execute()
		{
			_sendQueue = Context.Get<ISendQueue>();

			if(Context.LocalVars != null)
				Context.Get<IScriptLog>().Log(Context.Name, "send {0}".ToFormat(Token.Value), Context.LineNumber);

			_sendQueue.Add(Token.Value);

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}

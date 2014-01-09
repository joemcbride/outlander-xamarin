using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Outlander.Core.Client;
using Outlander.Core;

namespace Outlander.Core.Client.Scripting
{
	public class WeakMatchingTokenHandler : ITokenHandler
	{
		private readonly IGameState _gameState;
		private readonly IGameStream _gameStream;
		private readonly List<IWaitForMatcher> _matchers = new List<IWaitForMatcher>();
		private readonly GameStreamListener _listener;

		public WeakMatchingTokenHandler(IGameState gameState, IGameStream gameStream)
		{
			_gameState = gameState;
			_gameStream = gameStream;
			_listener = new GameStreamListener(tag => {
				Check(tag.Text);
			});
			_listener.Subscribe(_gameStream);
		}

		public virtual Task<CompletionEventArgs> Execute(ScriptContext context, Token token)
		{
			var matcher = BuildMatcher(context, token);

			context.CancelToken.Register(() => {
				if(_matchers.Contains(matcher))
				{
					_matchers.Remove(matcher);
				}
			});

			_matchers.Add(matcher);

			return matcher.TaskSource.Task;
		}

		protected virtual IWaitForMatcher BuildMatcher(ScriptContext context, Token token)
		{
			return new WaitForMatcher(context, token);
		}

		private void Check(string text)
		{
			_matchers.ToList().Apply(m => {

				if(m.Context.CancelToken.IsCancellationRequested)
					return;

				var result = m.Checkmatch(text);

				if(result.Success){
					_matchers.Remove(m);
					_gameState.DelayIfRoundtime(m.Context.CancelToken, ()=> {
						m.TaskSource.TrySetResult(result.Args);
					});
				}
			});
		}
	}
}

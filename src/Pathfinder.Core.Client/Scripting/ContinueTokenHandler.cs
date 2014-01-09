using System;

namespace Outlander.Core.Client
{
	public class ContinueTokenHandler : TokenHandler
	{
		private Action<ScriptContext, Token> _configure;

		public ContinueTokenHandler()
			: this(null)
		{
		}

		public ContinueTokenHandler(Action<ScriptContext, Token> configure)
		{
			_configure = configure;
		}

		protected override void execute()
		{
			if(_configure != null)
				_configure(Context, Token);

			TaskSource.SetResult(new CompletionEventArgs {});
		}
	}
}

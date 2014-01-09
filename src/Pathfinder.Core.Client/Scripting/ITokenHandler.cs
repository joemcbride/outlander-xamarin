using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Outlander.Core.Client
{
	public interface ITokenHandler
	{
		Task<CompletionEventArgs> Execute(ScriptContext context, Token token);
	}

	public class TokenHandler : ITokenHandler
	{
		protected Token Token { get; private set; }
		protected ScriptContext Context { get; private set; }
		protected TaskCompletionSource<CompletionEventArgs> TaskSource { get; private set; }

		public virtual Task<CompletionEventArgs> Execute(ScriptContext context, Token token)
		{
			Token = token;
			Context = context;
			TaskSource = new TaskCompletionSource<CompletionEventArgs>();

			execute();

			return TaskSource.Task;
		}

		protected virtual void execute()
		{
		}
	}
}

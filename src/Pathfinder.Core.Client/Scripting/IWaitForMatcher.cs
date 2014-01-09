using System;
using System.Threading.Tasks;

namespace Outlander.Core.Client.Scripting
{
	public interface IWaitForMatcher
	{
		Token Token { get; }
		ScriptContext Context { get; }
		TaskCompletionSource<CompletionEventArgs> TaskSource { get; }
		MatchResult Checkmatch(string text);
	}

	public class WaitForMatcher : IWaitForMatcher
	{
		public Token Token { get; private set; }
		public ScriptContext Context { get; private set; }
		public TaskCompletionSource<CompletionEventArgs> TaskSource { get; private set; }

		public WaitForMatcher(ScriptContext context, Token token)
		{
			Context = context;
			Token = token;
			TaskSource = new TaskCompletionSource<CompletionEventArgs>();
		}

		public virtual MatchResult Checkmatch(string text)
		{
			return text.Contains(Token.Value) ? MatchResult.True() : MatchResult.False();
		}
	}
}

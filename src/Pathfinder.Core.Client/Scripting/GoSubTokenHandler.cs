using System;
using System.Collections.Generic;

namespace Outlander.Core.Client
{
	public interface IGoSubStack
	{
		bool CanPop { get; }
		IEnumerable<GoSubItem> Stack();
		void Push(GoSubItem item);
		GoSubItem Pop();
		GoSubItem Peek();
	}

	public class GoSubStack : IGoSubStack
	{
		private readonly Stack<GoSubItem> _stack = new Stack<GoSubItem>();

		public bool CanPop
		{
			get { return _stack.Count > 0; }
		}

		public IEnumerable<GoSubItem> Stack()
		{
			return _stack.ToArray();
		}

		public void Push(GoSubItem item)
		{
			_stack.Push(item);
		}

		public GoSubItem Pop()
		{
			return _stack.Pop();
		}

		public GoSubItem Peek()
		{
			return _stack.Peek();
		}
	}

	public class GoSubItem
	{
		public string Label { get; private set; }
		public string[] Args { get; private set; }
		public int LineNumber { get; private set; }

		public static GoSubItem For(string label, int lineNumber, string[] args)
		{
			return new GoSubItem { Label = label, LineNumber = lineNumber, Args = args };
		}
	}

	public class GoSubTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var log = Context.Get<IScriptLog>();
			var replacer = Context.Get<IVariableReplacer>();
			var gotoToken = Token as GotoToken;

			Context.GoSubStack.Push(GoSubItem.For(gotoToken.Label, Context.LineNumber, gotoToken.Args));

			var replaced = replacer.Replace(gotoToken.Label, Context);

			if(Context.DebugLevel > 0)
			{
				log.Log(Context.Name, "gosub {0}".ToFormat(replaced), Context.LineNumber);
			}

			TaskSource.TrySetResult(new CompletionEventArgs { Goto = replaced, Args = gotoToken.Args });
		}
	}

	public class ReturnTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var log = Context.Get<IScriptLog>();

			if(Context.GoSubStack.CanPop && !string.IsNullOrWhiteSpace(Context.GoSubStack.Peek().Label))
			{
				var gosubItem = Context.GoSubStack.Pop();

				if(Context.DebugLevel > 0)
				{
					log.Log(Context.Name, "returning to line {0}".ToFormat(gosubItem.LineNumber + 1), Context.LineNumber);
				}

				Context.LineNumber = gosubItem.LineNumber;
				TaskSource.TrySetResult(new CompletionEventArgs { });
			}
			else
			{
				log.Log(Context.Name, "no label to return to", Context.LineNumber);

				TaskSource.TrySetCanceled();
			}
		}
	}

	public class GotoToken : Token
	{
		public string Label { get; set; }
		public string[] Args { get; set; }
	}
}

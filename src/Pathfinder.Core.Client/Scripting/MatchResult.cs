using System;

namespace Pathfinder.Core.Client.Scripting
{
	public class MatchResult
	{
		public bool Success { get; set; }
		public CompletionEventArgs Args { get; set; }

		public static MatchResult False()
		{
			return new MatchResult { Success = false, Args = null };
		}

		public static MatchResult True()
		{
			return True(null);
		}

		public static MatchResult True(string gotoDef)
		{
			return new MatchResult { Success = true, Args = new CompletionEventArgs { Goto = gotoDef } };
		}
	}
}

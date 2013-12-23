using System;
using System.Collections.Generic;

namespace Pathfinder.Core.Client.Scripting
{
	public class MatchWait
	{
		private IList<MatchToken> _matches = new List<MatchToken>();

		public IEnumerable<MatchToken> Matches()
		{
			return _matches;
		}

		public void Add(MatchToken item)
		{
			_matches.Add(item);
		}

		public void Clear()
		{
			_matches.Clear();
		}
	}

	public class MatchToken : Token
	{
		public string Pattern { get; set; }
		public string Goto { get; set; }
		public bool IsRegex { get; set; }

		public static MatchToken For(string pattern, string gotoDef, bool isRegex)
		{
			return new MatchToken { Pattern = pattern, Goto = gotoDef, IsRegex = isRegex };
		}
	}
}

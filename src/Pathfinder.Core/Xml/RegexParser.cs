using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core
{
	public abstract class RegexParser<TResult> : IParser where TResult : ParseResult
	{
		public string Pattern { get; set; }

		public bool Matches(string data)
		{
			return Regex.IsMatch(data, Pattern);
		}

		IEnumerable<ParseResult> IParser.Parse(string data)
		{
			return Parse(data);
		}

		public virtual IEnumerable<TResult> Parse(string data)
		{
			var matches = Regex.Matches(data, Pattern);

			var results = new List<TResult>();

			foreach(Match match in matches)
			{
				results.Add(BuildItem(match.Value));
			}

			return results;
		}

		protected abstract TResult BuildItem(string xml);
	}
	
}

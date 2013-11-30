using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core.Xml
{
	public class CompassParser : RegexParser<CompassResult>
	{
		private const string Match_Regex = "<compass(.+?)<\\/compass>";
		private const string Dir_Match_Regex = "<dir(.+?)\\/>";

		public CompassParser()
		{
			Pattern = Match_Regex;
		}

		public override IEnumerable<CompassResult> Parse(string data)
		{
			var matches = Regex.Matches(data, Pattern);

			var results = new List<CompassResult>();

			foreach(Match match in matches)
			{
				var dirMatches = Regex.Matches(match.Value, Dir_Match_Regex);
				foreach(Match dirMatch in dirMatches)
				{
					var item = BuildItem(dirMatch.Value);
					item.Matched = match.Value;
					results.Add(item);
				}
			}

			return results;
		}

		protected override CompassResult BuildItem(string xml)
		{
			return CompassResult.For(xml);
		}
	}

	public class CompassResult : ParseResult
	{
		public string Direction { get; set; }

		public static CompassResult For(string xml)
		{
			var result = new CompassResult();

			var element = XElement.Parse(xml);
			result.Direction = element.Attribute("value").Value;

			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core.Xml
{
	public class RoundtimeParser : RegexParser<RoundtimeResult>
	{
		private const string Match_Regex = "<roundTime(.+?)\\/>";

		public RoundtimeParser()
		{
			Pattern = Match_Regex;
		}

		protected override RoundtimeResult BuildItem(string xml)
		{
			return RoundtimeResult.For(xml);
		}
	}
}

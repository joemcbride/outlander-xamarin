using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core.Xml
{
	public class PromptParser : RegexParser<PromptResult>
    {
		private const string Match_Regex = "<prompt(.+?)<\\/prompt>";

		public PromptParser()
		{
			Pattern = Match_Regex;
		}

		protected override PromptResult BuildItem(string xml)
		{
			return PromptResult.For(xml);
		}
    }
}
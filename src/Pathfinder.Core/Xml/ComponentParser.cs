using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core.Xml
{
	public class ComponentParser : RegexParser<ComponentResult>
	{
		private const string Match_Regex = "<component(.+?)<\\/component>";

		public ComponentParser()
		{
			Pattern = Match_Regex;
		}

		protected override ComponentResult BuildItem(string xml)
		{
			return ComponentResult.For(xml);
		}
	}
}

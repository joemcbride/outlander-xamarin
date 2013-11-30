using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core
{

	public class RoundtimeResult : ParseResult
	{
		public DateTime? Finished { get; set; }

		public override bool IsValid()
		{
			return Finished.HasValue;
		}

		public static RoundtimeResult For(string xml)
		{
			var result = new RoundtimeResult();
			var element = XElement.Parse(xml.Trim());
			result.Finished = element.Attribute("value").Value.UnixTimeStampToDateTime();
			result.Matched = xml;
			return result;
		}
	}
}

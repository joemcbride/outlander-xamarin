using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core
{

	public class ComponentResult : ParseResult
	{
		public string Id { get; set; }
		public string Content { get; set; }

		public override bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(Id);
		}

		public static ComponentResult For(string xml)
		{
			var result = new ComponentResult ();

			var element = XElement.Parse(xml.Trim());
			result.Id = element.Attribute("id").Value;
			result.Content = element.Value.Trim();
			result.Matched = xml;

			return result;
		}
	}
	
}

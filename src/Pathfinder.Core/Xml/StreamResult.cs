using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core
{

	public class StreamResult : ParseResult
	{
		public string Type { get; set; }
		public string Value { get; set; }

		public static StreamResult For<TItem>(string xml)
			where TItem : StreamResult, new()
		{
			var item = new TItem();
			item.Matched = xml;

			var element = XElement.Parse(xml);
			item.Type = element.Attribute("id").Value;
			item.Value = element.Value;

			return item;
		}
	}
}

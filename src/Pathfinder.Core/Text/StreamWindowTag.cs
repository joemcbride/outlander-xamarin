using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{

	public class StreamWindowTag : Tag
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Subtitle { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			Id = element.Attribute("id").Value.Replace(" ", string.Empty).ToLower();
			element.Attribute("title").IfNotNull(e => Title = e.Value);
			element.Attribute("subtitle").IfNotNull(e => Subtitle = e.Value);
		}
	}
	
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{

	public class IndicatorTag : Tag
	{
		public string Id { get; set; }
		public string Value { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			var idValue = element.Attribute("id").Value;
			Id = idValue.Substring(4, idValue.Length - 4).Replace(" ", string.Empty).ToLower();
			Value = element.Attribute("visible").Value == "n" ? "0" : "1";
		}
	}
	
}

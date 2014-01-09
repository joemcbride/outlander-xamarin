using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Outlander.Core.Text
{
	public class PresetTag : Tag
	{
		public PresetTag()
		{
		}

		public PresetTag(string text)
			: base(text)
		{
		}

		public string Id { get; set; }
		public string Value { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			Id = element.Attribute("id").Value.Replace(" ", string.Empty).ToLower();
			Value = element.Value;
		}
	}
}

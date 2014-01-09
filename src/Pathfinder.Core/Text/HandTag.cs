using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Outlander.Core.Text
{
	public class LeftHandTag : HandTag
	{
	}

	public class RightHandTag : HandTag
	{
	}

	public class HandTag : Tag
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public string Noun { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			Name = element.Value;
			element.Attribute("exist").IfNotNull(x => Id = x.Value);
			element.Attribute("noun").IfNotNull(x => Noun = x.Value);
		}
	}
	
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Outlander.Core.Text
{
	public class AppTag : Tag
	{
		public string Game { get; set; }
		public string Character { get; private set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			Game = element.Attribute("game").Value;
			Character = element.Attribute("char").Value;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Outlander.Core.Text
{

	public class SpellTag : Tag
	{
		public string Spell { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			Spell = element.Value;
		}
	}
	
}

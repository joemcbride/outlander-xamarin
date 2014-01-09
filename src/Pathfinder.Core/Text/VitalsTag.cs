using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Outlander.Core.Text
{
	public class VitalsTag : Tag
	{
		public string Name { get; set; }
		public int Value { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			var progressBar = GetElement(element, "progressBar");

			if(progressBar != null)
			{
				Name = progressBar.Attribute("id").Value;
				Value = int.Parse(progressBar.Attribute("value").Value);
			}
		}

		private static XElement GetElement(XElement element, string xName)
		{
			return element
					.Elements()
					.Where(x => x.Name == XName.Get(xName))
					.FirstOrDefault();
		}
	}
}

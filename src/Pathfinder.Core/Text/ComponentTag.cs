using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class ComponentTag : Tag
	{
		const string EXP_Regex = @".+:\s+(\d+)\s(\d+)%\s(\w.*)?.*";

		private bool _isExp;

		public ComponentTag()
		{
		}

		public ComponentTag(string text)
			: base(text)
		{
		}

		public string Id { get; set; }
		public string Value { get; set; }

		public bool IsExp
		{
			get { return _isExp; }
		}

		protected override void OnTextSet()
		{
			try {
				var element = XElement.Parse(Text.Trim());
				var idValue = element.Attribute("id").Value;
				if(idValue.StartsWith("exp")) {
					_isExp = true;
					Id = idValue.Substring(4, idValue.Length - 4).Replace(" ", "_");
				}
				else {
					Id = idValue.Replace(" ", string.Empty).ToLower();
				}

				using (var reader = element.CreateReader())
				{
					reader.MoveToContent();
					Value = reader.ReadInnerXml().Trim().Replace(" />", "/>");
				}

				if(_isExp){
					Value = Regex.Replace(Value, "<[^>]*>", string.Empty).Trim();
					Value = Regex.Replace(Value, EXP_Regex, "$1 $2% $3");
				}
			} catch (Exception exc) {
				System.Diagnostics.Debug.WriteLine(exc);
			}
		}

		public static ComponentTag RoomExistsFor(ComponentTag tag)
		{
			tag.Value = tag.Value
				.Replace("<d>", string.Empty)
				.Replace("</d>", string.Empty)
				.Replace("<compass></compass>", string.Empty);

			return tag;
		}
	}
}

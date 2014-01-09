using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Outlander.Core.Text
{
	public class ComponentTag : Tag
	{
		const string EXP_Regex = @".+:\s+(\d+)\s(\d+)%\s(\w.*)?.*";

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
			get;
			private set;
		}

		public bool IsNewExp
		{
			get;
			private set;
		}

		protected override void OnTextSet()
		{
			try {
				bool isTdp = false;
				var element = XElement.Parse(Text.Trim());
				var idValue = element.Attribute("id").Value;
				if(idValue.StartsWith("exp") && !idValue.Equals("exp tdp")) {
					IsExp = true;
					Id = idValue.Substring(4, idValue.Length - 4).Replace(" ", "_");
				}
				else if(idValue.Equals("exp tdp"))
				{
					isTdp = true;
					Id = idValue.Substring(4, idValue.Length - 4);
				}
				else {
					Id = idValue.Replace(" ", string.Empty).ToLower();
				}

				using (var reader = element.CreateReader())
				{
					reader.MoveToContent();
					Value = reader.ReadInnerXml().Trim().Replace(" />", "/>");
				}

				if(IsExp) {
					IsNewExp = Value != null ? Value.Contains("whisper") : false;
					Value = Regex.Replace(Value, "<[^>]*>", string.Empty).Trim();
					Value = Regex.Replace(Value, EXP_Regex, "$1 $2% $3");
				}

				if(isTdp) {
					var match = Regex.Match(Value, "TDPs:\\s+(\\d+)");
					if(match.Success) {
						Value = match.Groups[1].Value;
					}
				}
			} catch (Exception exc) {
				System.Diagnostics.Debug.WriteLine(exc);
			}
		}

		public static ComponentTag RoomExistsFor(ComponentTag tag)
		{
			tag.Value = Regex.Replace(tag.Value, "<[^>]*>", string.Empty).TrimStart();

			return tag;
		}
	}
}

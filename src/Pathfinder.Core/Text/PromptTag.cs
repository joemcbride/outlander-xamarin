using System;
using System.Xml.Linq;

namespace Outlander.Core.Text
{
	public class PromptTag : Tag
	{
		public DateTime Time { get; set; }
		public string GameTime { get; set; }
		public string Prompt { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());

			GameTime = element.Attribute("time").Value;
			Time = GameTime.UnixTimeStampToDateTime();
			Prompt = element.Value;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PromptTag)obj);
		}

		public bool Equals(PromptTag other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return string.Equals(GameTime, other.GameTime) && string.Equals(Prompt, other.Prompt);
		}

		public override int GetHashCode()
		{
			return (GameTime != null ? GameTime.GetHashCode() : 0) ^
				(Prompt != null ? GameTime.GetHashCode() : 0);
		}
	}
}

using System;
using System.Xml.Linq;

namespace Pathfinder.Core.Text
{
	public class RoomNameTag : Tag
	{
		public string Name { get; set; }

		protected override void OnTextSet()
		{
			var changed = Text.Replace("<style id=\"\"/>", string.Empty);
			Name = changed.Substring(23, changed.Length - 23);
		}
	}
}

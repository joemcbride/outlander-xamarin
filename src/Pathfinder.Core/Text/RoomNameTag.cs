using System;
using System.Xml.Linq;

namespace Pathfinder.Core.Text
{
	public class RoomNameTag : Tag
	{
		public string Id { get; set; }
		public string Name { get; set; }

		protected override void OnTextSet()
		{
			var changed = Text.Replace("<style id=\"\"/>", string.Empty);
			Id = "roomname";
			Name = changed.Substring(23, changed.Length - 23);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class RoomDescriptionTag : PresetTag
	{
		public RoomDescriptionTag(string text)
			: base(text)
		{
		}

		public string Description { get; set; }

		protected override void OnTextSet()
		{
			base.OnTextSet();

			Description = Value;
		}

		public static RoomDescriptionTag For(PresetTag tag)
		{
			return new RoomDescriptionTag(tag.Text);
		}
	}
	
}

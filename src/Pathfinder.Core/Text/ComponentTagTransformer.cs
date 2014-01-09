using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Outlander.Core.Text
{
	public class ComponentTagTransformer : ITagTransformer
	{
		public bool Matches(Tag tag)
		{
			return tag.GetType() == typeof(ComponentTag);
		}

		public Tag Transform(Tag tag)
		{
			var compTag = (ComponentTag)tag;

			if(compTag.Id.Equals("roomexits"))
			{
				return ComponentTag.RoomExistsFor(compTag);
			}

			return compTag;
		}
	}
}

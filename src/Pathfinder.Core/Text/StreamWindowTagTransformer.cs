using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{

	public class StreamWindowTagTransformer : ITagTransformer
	{
		public bool Matches(Tag tag)
		{
			return tag.GetType() == typeof(StreamWindowTag);
		}

		public Tag Transform(Tag tag)
		{
			var streamTag = (StreamWindowTag)tag;

			if(streamTag.Id.Equals("room")){
				return new ComponentTag(string.Format("<component id='{0}'>{1}</component>", ComponentKeys.RoomTitle, streamTag.Subtitle.Replace(" - ", string.Empty)));
			}

			return streamTag;
		}
	}
	
}

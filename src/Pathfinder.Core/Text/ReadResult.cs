using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Pathfinder.Core.Text
{
	public class ReadResult
	{
		private List<Tag> _tags = new List<Tag>();

		public IEnumerable<Tag> Tags
		{
			get { return _tags; }
		}

		public bool Stop { get; set; }

		public Chunk Chunk { get; set; }

		public void AddTag(Tag tag)
		{
			_tags.Add(tag);
		}

		public void AddTags(IEnumerable<Tag> tags)
		{
			_tags.AddRange(tags);
		}
	}
}

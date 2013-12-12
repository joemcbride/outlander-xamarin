using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class OutputChunkReader : IChunkReader
	{
		private ChunkReader<Tag> _reader;

		public OutputChunkReader()
		{
			_reader = new ChunkReader<Tag>("<output class=\"mono\"", "<output class=\"\"", true);
			_reader.Append = (builder, result, tag) => {
				builder.Append(tag.Text);
				//result.AddTag(tag);
				return 0;
			};
		}

		public ReadResult Read(Chunk chunk)
		{
			return _reader.Read(chunk);
		}
	}
}

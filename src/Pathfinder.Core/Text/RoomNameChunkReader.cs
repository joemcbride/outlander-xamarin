using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{

	public class RoomNameChunkReader : IChunkReader
	{
		private ChunkReader<RoomNameTag> _reader;

		public RoomNameChunkReader()
		{
			_reader = new ChunkReader<RoomNameTag>("<style id=\"roomName\"", "<style id=\"\"", true);
			_reader.Append = (builder, result, tag) => {
				builder.Append(tag.Name);
				result.AddTag(tag);
				return 0;
			};
		}

		public ReadResult Read(Chunk chunk)
		{
			return _reader.Read(chunk);
		}
	}
	
}

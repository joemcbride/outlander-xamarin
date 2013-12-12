using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class PresetChunkReader : IChunkReader
	{
		private ChunkReader<PresetTag> _reader;

		public PresetChunkReader()
		{
			_reader = new ChunkReader<PresetTag>("<preset", "</preset");
			_reader.Append = (builder, result, tag) => {
				var skip = 0;
				if(tag.Id.Equals("roomdesc")) {
					builder.Append(tag.Value + "\n");
					skip = 2;
				}

				if(tag.Id.Equals("speech")) {
					builder.Append(tag.Value);
				}

				result.AddTag(tag);

				return skip;
			};
		}

		public ReadResult Read(Chunk chunk)
		{
			return _reader.Read(chunk);
		}
	}
}

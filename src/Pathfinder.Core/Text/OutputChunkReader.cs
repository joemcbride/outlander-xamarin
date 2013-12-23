using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class OutputChunkReader : IChunkReader
	{
		private const string Start_Tag = "<output class=\"mono\"/>";
		private const string End_Tag = "<output class=\"\"/>";

		private ChunkReader<Tag> _reader;

		public OutputChunkReader()
		{
			_reader = new ChunkReader<Tag>("<output class=\"mono\"", "<output class=\"\"", true);
			_reader.Append = (builder, result, tag) => {
				return 0;
			};
			_reader.AppendPartial = (builder, state, result) => {
				if(state.Text.Length > 0) {
					var newData = state.Text.ToString().Substring(state.DequedTextIndex, state.Text.Length - state.DequedTextIndex);
					state.DequedText.Append(newData);
					state.DequedTextIndex = state.Text.Length;

					if(!string.IsNullOrWhiteSpace(newData)){
						if(!newData.StartsWith(Start_Tag)){
							newData = Start_Tag + newData;
						}
						if(!newData.EndsWith(End_Tag)){
							newData += End_Tag;
						}
						builder.Append(newData);
					}
				}
			};
		}

		public ReadResult Read(Chunk chunk)
		{
			return _reader.Read(chunk);
		}
	}
}

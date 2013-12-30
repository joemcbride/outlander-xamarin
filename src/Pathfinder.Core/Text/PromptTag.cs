using System;
using System.Xml.Linq;

namespace Pathfinder.Core.Text
{
	public class PromptTag : Tag
	{
		public DateTime Time { get; set; }
		public string GameTime { get; set; }
		public string Prompt { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());

			GameTime = element.Attribute("time").Value;
			Time = GameTime.UnixTimeStampToDateTime();
			Prompt = element.Value;
		}
	}

	public class PromptChunkReader : IChunkReader
	{
		private ChunkReader<PromptTag> _reader;

		public PromptChunkReader()
		{
			_reader = new ChunkReader<PromptTag>("<prompt", "</prompt");
			_reader.Append = (builder, result, tag) => {
				builder.Append(tag.Prompt);
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

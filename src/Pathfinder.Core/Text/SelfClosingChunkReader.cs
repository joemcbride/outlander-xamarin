using System;
using System.Text;

namespace Pathfinder.Core.Text
{
	public class SelfClosingChunkReader<TTag> : IChunkReader where TTag : Tag, new()
	{
		private ReadState _state = new ReadState();
		private readonly string _startTag;
		private readonly string _endTag;

		public SelfClosingChunkReader(string startTag)
		{
			_startTag = startTag;
			_endTag = "/>";
		}

		public ReadResult Read(Chunk chunk)
		{
			var result = new ReadResult();

			var builder = new StringBuilder();

			var text = chunk.Text;

			for (int i = 0; i < text.Length; i++) {
				if (text[i] == '<') {
					_state.Tracking = true;
				}

				if (_state.Tracking) {
					_state.Text.Append(text[i]);
				} else {
					builder.Append(text[i]);
				}

				var currentText = _state.Text.ToString();

				if (_state.Tracking
					&& (currentText.Length == 3 && !currentText.StartsWith(_startTag.Substring(0, 3))
						|| (currentText.Length >= _startTag.Length && !currentText.StartsWith(_startTag))))
				{
					_state.Tracking = false;
					builder.Append(_state.Text.ToString());
					_state.Text.Clear();
				} else if (currentText.StartsWith(_startTag)) {
					_state.WaitForPop = true;
				}

				if(_state.Tracking && currentText.EndsWith(_endTag)){
					_state.WaitForEnd = false;
					_state.WaitForPop = false;
					_state.Tracking = false;
					_state.Text.Clear();

					result.AddTag(Tag.For<TTag>(currentText));
				}
			}

			if (builder.Length > 0)
			{
				result.Chunk = Chunk.For(builder.ToString());
			}

			return result;
		}
	}
}

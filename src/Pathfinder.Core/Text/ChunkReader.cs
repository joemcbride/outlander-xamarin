using System;
using System.Text;
using System.Collections.Generic;

namespace Pathfinder.Core.Text
{
	public class ChunkReader
	{
		private ReadState _state = new ReadState();
		private Queue<Chunk> _chunks = new Queue<Chunk>();

		public ChunkReader()
		{
			Tags = new List<Chunk>();
			Push = new List<Chunk>();
		}

		public bool InStream { get; private set; }
		public List<Chunk> Tags { get; set; }
		public List<Chunk> Push { get; set; }

		public void Add(Chunk chunk)
		{
			_chunks.Enqueue(chunk);
		}

		public void Read()
		{
			if (_chunks.Count == 0)
				return;

			const string StartTag = "<pushS";
			const string EndTag = "<popStream";

			var builder = new StringBuilder();

			var chunk = _chunks.Dequeue();
			var text = chunk.Text;

			for (int i = 0; i < text.Length; i++) {
				if(text[i] == '<') {
					_state.Tracking = true;
					_state.TrackingIdx = i;
				}

				if(_state.Tracking){
					_state.Text.Append(text[i]);
				}
				else {
					builder.Append(text[i]);
				}

				var currentText = _state.Text.ToString();

				if(_state.Tracking && currentText.Length >= 6 && !currentText.StartsWith(StartTag)){
					_state.Tracking = false;
					_state.TrackingIdx = -1;
					builder.Append(_state.Text.ToString());
					_state.Text.Clear();
				} else if(currentText.StartsWith(StartTag)) {
					_state.WaitForPop = true;
				}

				if(_state.WaitForEnd && currentText.EndsWith("/>")){
					_state.WaitForEnd = false;
					_state.WaitForPop = false;
					_state.Tracking = false;
					_state.TrackingIdx = -1;
					_state.Text.Clear();

					Tags.Add(Chunk.For(currentText));
				}

				if(_state.WaitForPop && currentText.EndsWith(EndTag)){
					_state.WaitForEnd = true;
				}
			}

			var newChunk = Chunk.For(builder.ToString());
			Push.Add(newChunk);
		}

		private class ReadState
		{
			public StringBuilder Text = new StringBuilder();
			public int TrackingIdx = 0;
			public bool Tracking = false;
			public bool WaitForPop = false;
			public bool WaitForEnd = false;
		}
	}

	public class Chunk
	{
		public Chunk(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }

		public static Chunk For(string text, int start = 0, int length = -1)
		{
			if (length == -1)
				length = text.Length;

			return new Chunk(text.Substring(start, length));
		}
	}
}

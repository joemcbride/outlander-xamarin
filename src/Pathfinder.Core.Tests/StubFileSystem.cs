using System;

namespace Pathfinder.Core.Tests
{
	public class StubFileSystem : IFileSystem
	{
		private bool _exists;
		private string _text;

		public string LastSaveContent { get; set; }
		public string LastSavePath { get; set; }

		public bool Exists(string path)
		{
			return _exists;
		}

		public string ReadAllText(string path)
		{
			return _text;
		}

		public void Save(string content, string path)
		{
			LastSaveContent = content;
			LastSavePath = path;
		}

		public void StubExists(bool exists)
		{
			_exists = exists;
		}

		public void StubReadAllText(string text)
		{
			_text = text;
		}
	}
}

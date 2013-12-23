using System;
using System.IO;

namespace Pathfinder.Core
{
	public interface IFileSystem
	{
		bool Exists(string path);
		string ReadAllText(string path);
	}

	public class FileSystem : IFileSystem
	{
		public bool Exists(string path)
		{
			return File.Exists(path);
		}

		public string ReadAllText(string path)
		{
			return File.ReadAllText(path);
		}
	}
}

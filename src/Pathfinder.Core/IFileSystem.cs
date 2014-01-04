using System;
using System.IO;
using System.Text;

namespace Pathfinder.Core
{
	public interface IFileSystem
	{
		bool Exists(string path);
		string ReadAllText(string path);
		void Save(string content, string path);
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

		public void Save(string content, string path)
		{
			File.WriteAllText(path, content, Encoding.UTF8);
		}
	}

	public interface IDirectorySystem
	{
		bool Exists(string path);
		void Create(string path);
	}

	public class DirectorySystem : IDirectorySystem
	{
		public bool Exists(string path)
		{
			return Directory.Exists(path);
		}

		public void Create(string path)
		{
			Directory.CreateDirectory(path);
		}
	}
}

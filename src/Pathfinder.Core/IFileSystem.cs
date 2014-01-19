using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Outlander.Core
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
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Outlander.Core
{
	public interface IDirectorySystem
	{
		bool Exists(string path);
		void Create(string path);
		void Delete(string path);
		IEnumerable<string> Directories(string path);
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

		public void Delete(string path)
		{
			Directory.Delete(path, true);
		}

		public IEnumerable<string> Directories(string path)
		{
			var dirs = Directory.EnumerateDirectories(path).ToList();
			return dirs;
		}
	}
}

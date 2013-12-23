using System;
using System.IO;

namespace Pathfinder.Core
{
	public class BuildAppDirectories
	{
		public void Build(AppSettings settings)
		{
			CreateIfNotExists(settings.HomeDirectory);
			CreateIfNotExists(Path.Combine(settings.HomeDirectory, AppSettings.LogFolder));
			CreateIfNotExists(Path.Combine(settings.HomeDirectory, AppSettings.ScriptsFolder));
		}

		private void CreateIfNotExists(string dir)
		{
			if(!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
		}
	}
}

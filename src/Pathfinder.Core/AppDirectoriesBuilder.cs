using System;
using System.IO;

namespace Pathfinder.Core
{
	public class AppDirectoriesBuilder
	{
		private readonly IDirectorySystem _directorySystem;
		private readonly AppSettings _settings;

		public AppDirectoriesBuilder(IDirectorySystem directorySystem, AppSettings settings)
		{
			_directorySystem = directorySystem;
			_settings = settings;
		}

		public void Build()
		{
			CreateDirectoryIfNotExists(_settings.HomeDirectory);
			CreateDirectoryIfNotExists(Path.Combine(_settings.HomeDirectory, AppSettings.LogFolder));
			CreateDirectoryIfNotExists(Path.Combine(_settings.HomeDirectory, AppSettings.ScriptsFolder));
			CreateDirectoryIfNotExists(Path.Combine(_settings.HomeDirectory, AppSettings.ConfigFolder, AppSettings.ProfilesFolder));

			BuildProfile("Default");
		}

		public void BuildProfile(string profile)
		{
			var root = Path.Combine(_settings.HomeDirectory, AppSettings.ConfigFolder, AppSettings.ProfilesFolder, profile);

			CreateDirectoryIfNotExists(root);
		}

		private void CreateDirectoryIfNotExists(string dir)
		{
			if(!_directorySystem.Exists(dir))
			{
				_directorySystem.Create(dir);
			}
		}
	}
}

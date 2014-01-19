using System;
using System.IO;
using System.Linq;
using Outlander.Core;

namespace Outlander.Core.Client
{
	public interface IAppSettingsLoader
	{
		void Load();
		void SaveVariables();

		void LoadConfig();
		void SaveConfig();
	}

	public class AppSettingsLoader : IAppSettingsLoader
	{
		private readonly IAppDirectoriesBuilder _directoryBuilder;
		private readonly IFileSystem _fileSystem;
		private readonly IConfigLoader _configLoader;
		private readonly IVariablesLoader _variablesLoader;
		private readonly IHighlightsLoader _highlightsLoader;
		private readonly IGameState _gameState;
		private readonly IServiceLocator _services;
		private readonly AppSettings _settings;

		public AppSettingsLoader(
			IAppDirectoriesBuilder directoryBuilder,
			IFileSystem fileSystem,
			IConfigLoader configLoader,
			IVariablesLoader variablesLoader,
			IHighlightsLoader highlightsLoader,
			IGameState gameState,
			IServiceLocator services,
			AppSettings settings)
		{
			_directoryBuilder = directoryBuilder;
			_fileSystem = fileSystem;
			_configLoader = configLoader;
			_variablesLoader = variablesLoader;
			_highlightsLoader = highlightsLoader;
			_gameState = gameState;
			_services = services;
			_settings = settings;
		}

		public void Load()
		{
			LoadConfig();

			_directoryBuilder.BuildProfile(_settings.Profile);

			LoadVariables();
			LoadHighlights();
		}

		public void LoadConfig()
		{
			var configFile = Path.Combine(
				                 _settings.HomeDirectory,
				                 AppSettings.ConfigFolder,
				                 AppSettings.ConfigFileName);

			if(_fileSystem.Exists(configFile))
			{
				_configLoader.Load(configFile);
			}

			_configLoader.Save(configFile);
		}

		public void SaveConfig()
		{
			var configFile = Path.Combine(
				_settings.HomeDirectory,
				AppSettings.ConfigFolder,
				AppSettings.ConfigFileName);

			_configLoader.Save(configFile);
		}

		public void SaveVariables()
		{
			var variablesFile = PathForFile(AppSettings.VariablesFileName);

			var values = _gameState.GlobalVars().Values().OrderBy(x => x.Key).ToDictionary(x => x.Key, x=> x.Value);
			_variablesLoader.Save(values, variablesFile);
		}

		private void LoadVariables()
		{
			var variablesFile = PathForFile(AppSettings.VariablesFileName);

			if(_fileSystem.Exists(variablesFile))
			{
				_variablesLoader
					.Load(variablesFile)
					.Apply(v => _gameState.Set(v.Key, v.Value));
			}
		}

		private void LoadHighlights()
		{
			var file = PathForFile(AppSettings.HighlightsFileName);

			if(_fileSystem.Exists(file))
			{
				_highlightsLoader
					.Load(file)
					.Apply(h => {
						_services.Instance<IHighlighter>(SimpleHighlighter.For(h.Pattern, h.Color));
					});
			}
		}

		private string PathForFile(string file)
		{
			return Path.Combine(
				_settings.HomeDirectory,
				AppSettings.ConfigFolder,
				AppSettings.ProfilesFolder,
				_settings.Profile,
				file);
		}
	}
}

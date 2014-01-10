using System;
using System.IO;
using System.Linq;
using Outlander.Core;

namespace Outlander.Core.Client
{
	public class AppSettingsLoader
	{
		private readonly IFileSystem _fileSystem;
		private readonly IVariablesLoader _variablesLoader;
		private readonly IHighlightsLoader _highlightsLoader;
		private readonly IGameState _gameState;
		private readonly IServiceLocator _services;
		private readonly AppSettings _settings;

		private const string Profile = "Default";

		public AppSettingsLoader(
			IFileSystem fileSystem,
			IVariablesLoader variablesLoader,
			IHighlightsLoader highlightsLoader,
			IGameState gameState,
			IServiceLocator services,
			AppSettings settings)
		{
			_fileSystem = fileSystem;
			_variablesLoader = variablesLoader;
			_highlightsLoader = highlightsLoader;
			_gameState = gameState;
			_services = services;
			_settings = settings;
		}

		public void Load()
		{
			LoadVariables();
			LoadHighlights();
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
				Profile,
				file);
		}
	}
}

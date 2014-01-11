using System;
using System.IO;
using Outlander.Core.Authentication;
using Outlander.Core.Text;
using Outlander.Core.Client;
using Outlander.Core.Client.Scripting;

namespace Outlander.Core
{
	public class Bootstrapper
	{
		private SimpleContainer _container;

		public IServiceLocator ServiceLocator()
		{
			return _container.GetInstance<IServiceLocator>();
		}

		public IGameServer Build()
		{
			ConfigureContainer();

			return _container.GetInstance<IGameServer>();
		}

		private void ConfigureContainer()
		{
			_container = new SimpleContainer();

			var appSettings = new AppSettings();

			var services = new ServiceLocator(_container);

			_container.Instance<AppSettings>(appSettings);
			_container.Singleton<IFileSystem, FileSystem>();
			_container.Singleton<IDirectorySystem, DirectorySystem>();
			_container.Instance<IServiceLocator>(services);
			_container.PerRequest<IAsyncSocket, AsyncSocket>();
			_container.PerRequest<IAuthenticationServer, AuthenticationServer>();
			_container.Singleton<IGameParser, NewGameParser>();
			_container.Singleton<IGameState, SimpleGameState>();
			_container.Singleton<IGameServer, SimpleGameServer>();
			_container.Singleton<IScriptLoader, ScriptLoader>();
			_container.Singleton<IScriptRunner, ScriptRunner>();
			_container.Singleton<IRoundtimeHandler, RoundtimeHandler>();
			_container.Singleton<WaitForTokenHandler>();
			_container.Singleton<WaitForReTokenHandler>();
			_container.Singleton<MatchWaitTokenHandler>();

			_container.PerRequest<ITagTransformer, ComponentTagTransformer>();
			_container.PerRequest<ITagTransformer, StreamWindowTagTransformer>();

			_container.PerRequest<IIfBlockExecuter, IfBlockExecuter>();
			_container.Singleton<IIfBlocksParser, IfBlocksParser>();

			_container.PerRequest<IScript, Script>();
			_container.Singleton<IScriptLog, ScriptLog>();

			_container.PerRequest<IVariableReplacer, VariableReplacer>();
			_container.Singleton<ICommandProcessor, CommandProcessor>();

			_container.Singleton<IGameStream, GameStream>();

			_container.Singleton<ISendQueue, SendQueue>();

			_container.Singleton<IConfigLoader, ConfigLoader>();
			_container.Singleton<IVariablesLoader, VariablesLoader>();
			_container.Singleton<IHighlightsLoader, HighlightsLoader>();
			_container.Singleton<IAppDirectoriesBuilder, AppDirectoriesBuilder>();
			_container.Singleton<IAppSettingsLoader, AppSettingsLoader>();

			var now = DateTime.Now.ToString("s");
			var logFileName = string.Format("{0}-log.txt", now);
			var errorsFileName = string.Format("{0}-errors.txt", now);

			var logger = new SimpleFileLogger(logFileName, errorsFileName, services);

			var compositeLog = new CompositeLog(new ILog[]{ logger });

			_container.Instance<ILog>(compositeLog);

			SetupHighlights();
		}

		private void SetupHighlights()
		{
			var highlightSettings = HighlightSettings.Default();
			_container.Instance(highlightSettings);

			var whisper = highlightSettings.Get(HighlightKeys.Whisper);

			_container.PerRequest<Highlights>();
			_container.PerRequest<IHighlighter, MonoHighlighter>();
			_container.PerRequest<IHighlighter, RoomNameHighlighter>();
			_container.PerRequest<IHighlighter, BoldHighlighter>();

			_container.Instance<IHighlighter>(new SimpleHighlighter("You say|You ask|You exclaim|says|whispers|asks|exclaims", HighlightKeys.Whisper, whisper.Color, whisper.Mono));
		}
	}
}

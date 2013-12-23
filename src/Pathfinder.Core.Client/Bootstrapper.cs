using System;
using System.IO;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;
using Pathfinder.Core.Client;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core
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
			_container.Instance<IServiceLocator>(services);
			_container.PerRequest<IAsyncSocket, AsyncSocket>();
			_container.PerRequest<IAuthenticationServer, AuthenticationServer>();
			_container.Singleton<IGameParser, NewGameParser>();
			_container.Singleton<IGameState, SimpleGameState>();
			_container.Singleton<IGameServer, SimpleGameServer>();
			_container.Singleton<IScriptLoader, ScriptLoader>();
			_container.Singleton<IScriptRunner, ScriptRunner>();
			_container.Singleton<IRoundtimeHandler, RoundtimeHandler>();

			_container.PerRequest<ITagTransformer, ComponentTagTransformer>();
			_container.PerRequest<ITagTransformer, StreamWindowTagTransformer>();

			_container.PerRequest<IScript, Script>();
			_container.Singleton<IScriptLog, ScriptLog>();

			_container.PerRequest<IVariableReplacer, VariableReplacer>();
			_container.Singleton<ICommandProcessor, CommandProcessor>();

			var now = DateTime.Now.ToString("s");
			var logFileName = string.Format("{0}-log.txt", now);
			var errorsFileName = string.Format("{0}-errors.txt", now);

			var logger = new SimpleFileLogger(logFileName, errorsFileName, services);

			var compositeLog = new CompositeLog(new ILog[]{ logger });

			_container.Instance<ILog>(compositeLog);

			var settings = HighlightSettings.Default();

			_container.Instance(settings);

			_container.PerRequest<Highlights>();
			_container.PerRequest<IHighlighter, MonoHighlighter>();
			_container.PerRequest<IHighlighter, RoomNameHighlighter>();
			_container.PerRequest<IHighlighter, BoldHighlighter>();
			_container.Instance<IHighlighter>(new SimpleHighlighter("says|whispers", HighlightKeys.Whisper, settings));

			_container.Instance<IHighlighter>(new SimpleHighlighter("Tayek", "Tayek", settings));
			_container.Instance<IHighlighter>(new SimpleHighlighter("steelsilk", "steelsilk", settings));
			_container.Instance<IHighlighter>(new SimpleHighlighter("^You've gained a new rank.*$", "newrank", settings));
			_container.Instance<IHighlighter>(new SimpleHighlighter("^Your formation of a targeting pattern.*$", "target", settings));
			_container.Instance<IHighlighter>(new SimpleHighlighter("^(You begin to target|You begin to weave mana lines into a target pattern).*$", "target", settings));


			settings.Add(new HighlightSetting{ Id = "newrank", Color = "#0000FF"  });
			settings.Add(new HighlightSetting{ Id = "target", Color = "#33FF08" });
			settings.Add(new HighlightSetting{ Id = "Tayek", Color = "#0000FF"  });
			settings.Add(new HighlightSetting{ Id = "steelsilk", Color = "#296B00"  });
		}
	}
}

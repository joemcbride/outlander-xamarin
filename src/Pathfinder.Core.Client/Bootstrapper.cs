using System;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;
using Pathfinder.Core.Client;

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

			//IoC.BuildUp = container.BuildUp;
			//IoC.GetInstance = container.GetInstance;
			//IoC.GetAllInstances = container.GetAllInstances;

			_container.Instance<IServiceLocator>(new ServiceLocator(_container));
			_container.PerRequest<IAsyncSocket, AsyncSocket>();
			_container.PerRequest<IAuthenticationServer, AuthenticationServer>();
			_container.Singleton<IGameParser, NewGameParser>();
			_container.Singleton<IGameState, SimpleGameState>();
			_container.Singleton<IGameServer, SimpleGameServer>();

			_container.PerRequest<ITagTransformer, ComponentTagTransformer>();
			_container.PerRequest<ITagTransformer, StreamWindowTagTransformer>();

			var now = DateTime.Now.ToString("s");
			var logFileName = string.Format("{0}-log.txt", now);
			var errorsFileName = string.Format("{0}-errors.txt", now);

			//var logger = new SimpleFileLogger(logFileName, errorsFileName);

			var compositeLog = new CompositeLog(new ILog[]{ new DebugLog() });

			_container.Instance<ILog>(compositeLog);

			var settings = HighlightSettings.Default();

			_container.Instance(settings);

			_container.PerRequest<Highlights>();
			_container.PerRequest<IHighlighter, MonoHighlighter>();
			_container.PerRequest<IHighlighter, RoomNameHighlighter>();
			_container.PerRequest<IHighlighter, BoldHighlighter>();
			_container.Instance<IHighlighter>(new SimpleHighlighter("Tayek", "Tayek", settings));
			_container.Instance<IHighlighter>(new SimpleHighlighter("steelsilk", "steelsilk", settings));

			settings.Add(new HighlightSetting{ Id = "Tayek", Color = "#0000FF"  });
			settings.Add(new HighlightSetting{ Id = "steelsilk", Color = "#296B00"  });
		}
	}
}

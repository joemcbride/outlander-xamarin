using System;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;
using Pathfinder.Core.Client;

namespace Pathfinder.Core
{
	public class Bootstrapper
	{
		public IGameServer Build()
		{
			ConfigureContainer();

			return IoC.Get<IGameServer>();
		}

		private void ConfigureContainer()
		{
			var container = new SimpleContainer();

			IoC.BuildUp = container.BuildUp;
			IoC.GetInstance = container.GetInstance;
			IoC.GetAllInstances = container.GetAllInstances;

			container.Singleton<IServiceLocator, ServiceLocator>();
			container.PerRequest<IAsyncSocket, AsyncSocket>();
			container.PerRequest<IAuthenticationServer, AuthenticationServer>();
			container.PerRequest<IGameParser, NewGameParser>();
			container.Singleton<IGameState, SimpleGameState>();
			container.Singleton<IGameServer, SimpleGameServer>();

			container.PerRequest<ITagTransformer, ComponentTagTransformer>();

			var now = DateTime.Now.ToString("s");
			var logFileName = string.Format("{0}-log.txt", now);
			var errorsFileName = string.Format("{0}-errors.txt", now);

			var logger = new SimpleFileLogger(logFileName, errorsFileName);

			var compositeLog = new CompositeLog(new ILog[]{ logger });

			container.Instance<ILog>(compositeLog);

			container.Instance(HighlightSettings.Default());

			container.PerRequest<Highlights>();
			container.PerRequest<IHighlighter, MonoHighlighter>();
			container.PerRequest<IHighlighter, RoomNameHighlighter>();
			container.PerRequest<IHighlighter, BoldHighlighter>();
		}
	}
}

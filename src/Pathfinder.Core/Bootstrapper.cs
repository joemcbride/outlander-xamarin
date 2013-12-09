using System;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

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

			container.PerRequest<IServiceLocator, ServiceLocator>();
			container.PerRequest<IAsyncSocket, AsyncSocket>();
			container.PerRequest<IAuthenticationServer, AuthenticationServer>();
			container.PerRequest<IGameParser, NewGameParser>();
			container.PerRequest<ISimpleGameState, SimpleGameState>();
			container.PerRequest<IGameServer, SimpleGameServer>();

			container.PerRequest<ITagTransformer, ComponentTagTransformer>();

			var now = DateTime.Now.ToString("s");
			var logFileName = string.Format("{0}-log.txt", now);
			var errorsFileName = string.Format("{0}-errors.txt", now);

			var logger = new SimpleFileLogger(logFileName, errorsFileName);

			var compositeLog = new CompositeLog(new ILog[]{ logger });

			container.Instance<ILog>(compositeLog);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Client
{
	public class MoveTokenHandler : TokenHandler
	{
		private RoomTagReporter _tracker;

		protected override void execute()
		{
			var processor = Context.Get<ICommandProcessor>();
			var replaced = processor.Eval(Token.Value, Context);

			Context.Get<IScriptLog>().Log(Context.Name, "waitformove " + replaced, Context.LineNumber);

			Context.CancelToken.Register(() => {
				_tracker.IfNotNull(t => t.Unsubscribe());
			});

			_tracker = new RoomTagReporter(TaskSource);
			_tracker.Subscribe(Context.Get<IGameState>().TagTracker);

			processor.Process(replaced, Context);
		}
	}
}

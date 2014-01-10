using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Outlander.Core.Text;
using Outlander.Core;

namespace Outlander.Core.Client
{
	public class NextroomTokenHandler : TokenHandler
	{
		private RoomTagReporter _tracker;

		protected override void execute()
		{
			if(Context.DebugLevel > 0) {
				Context.Get<IScriptLog>().Log(Context.Name, "waiting for next room", Context.LineNumber);
			}

			Context.CancelToken.Register(() => {
				_tracker.IfNotNull(t => t.Unsubscribe());
			});

			_tracker = new RoomTagReporter(TaskSource);
			_tracker.Subscribe(Context.Get<IGameState>().TagTracker);
		}
	}

	public class RoomTagReporter : DataReporter<IEnumerable<Tag>>
	{
		private TaskCompletionSource<CompletionEventArgs> _taskSource;

		public RoomTagReporter(TaskCompletionSource<CompletionEventArgs> taskSource)
			: base(Guid.NewGuid().ToString())
		{
			_taskSource = taskSource;
		}

		public override void OnNext(IEnumerable<Tag> tags)
		{
			if(tags.Any(t => t is RoomNameTag)) {
				Unsubscribe();
				_taskSource.TrySetResult(new CompletionEventArgs());
			}
		}
	}
}

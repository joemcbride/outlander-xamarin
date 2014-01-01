using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class ActionToken : Token
	{
		public string Action { get; set; }
		public string When { get; set; }
	}

	public class ActionTokenHandler : TokenHandler
	{
		private PatternReporter _reportPattern;

		protected override void execute()
		{
			var token = Token as ActionToken;

			var reporter = new PatternReporter(token, null);

			var gameState = Context.Get<IGameState>();
			gameState.TextTracker.Subscribe(reporter);
		}
	}

	public class PatternReporter : DataReporter<string>
	{
		private readonly DataTracker<ActionToken> _tracker;
		private readonly ActionToken _token;

		public PatternReporter(ActionToken token, DataTracker<ActionToken> tracker)
			: base(Guid.NewGuid().ToString())
		{
			_token = token;
			_tracker = tracker;
		}

		public override void OnNext(string item)
		{
			if(Regex.IsMatch(item))
			{
				_tracker.Publish(_token);
			}
		}
	}
}

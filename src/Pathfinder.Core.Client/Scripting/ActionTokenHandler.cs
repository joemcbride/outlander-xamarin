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

	public class ActionContext
	{
		public string ScriptName { get; set; }
		public int LineNumber { get; set; }
		public ScriptContext ScriptContext { get; set; }
		public ActionToken Token { get; set; }
	}

	public class ActionTokenHandler : TokenHandler
	{
		private readonly ActionContext _actionContext;
		private readonly IDataTracker<ActionContext> _tracker;
		private PatternReporter _reportPattern;

		public ActionTokenHandler(ActionContext context, IDataTracker<ActionContext> tracker)
		{
			_actionContext = context;
			_tracker = tracker;
		}

		protected override void execute()
		{
			//Context.Get<IScriptLog>().Log(Context.Name, "", token.LineNumber);

			_reportPattern = new PatternReporter(_actionContext, _tracker);

			var gameState = Context.Get<IGameState>();
			gameState.TextTracker.Subscribe(_reportPattern);

			Context.CancelToken.Register(() => {
				_reportPattern.Unsubscribe();
			});
		}
	}

	public class PatternReporter : DataReporter<string>
	{
		private readonly IDataTracker<ActionContext> _tracker;
		private readonly ActionContext _token;

		public PatternReporter(ActionContext token, IDataTracker<ActionContext> tracker)
			: base(Guid.NewGuid().ToString())
		{
			_token = token;
			_tracker = tracker;
		}

		public override void OnNext(string item)
		{
			if(Regex.IsMatch(item, _token.Token.When))
			{
				_tracker.Publish(_token);
			}
		}
	}
}

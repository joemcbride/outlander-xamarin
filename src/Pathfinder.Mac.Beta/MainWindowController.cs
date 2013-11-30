using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Pathfinder.Core;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Xml;

namespace Pathfinder.Mac.Beta
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		private SimpleContainer _container;

		private GameServer _gameServer;
		private System.Timers.Timer _timer;

		private PromptResult _lastPrompt;

		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		// Call to load from the XIB/NIB file
		public MainWindowController() : base("MainWindow")
		{
			Initialize();
		}
		// Shared initialization code
		void Initialize()
		{
			_container = new SimpleContainer();

			IoC.BuildUp = _container.BuildUp;
			IoC.GetInstance = _container.GetInstance;
			IoC.GetAllInstances = _container.GetAllInstances;

			_container.PerRequest<ITransformer, PopStreamTransformer>();
			_container.PerRequest<IParser, PromptParser>();
			_container.PerRequest<IParser, GenericStreamParser>();
			_container.PerRequest<IParser, ComponentParser>();
			_container.PerRequest<IParser, RoundtimeParser>();
			_container.PerRequest<IParser, LeftHandParser>();
			_container.PerRequest<IParser, RightHandParser>();
			_container.PerRequest<IParser, VitalsParser>();
			_container.PerRequest<IParser, CompassParser>();

			_container.PerRequest<GameParser>();

			_lastPrompt = new PromptResult();
			_lastPrompt.Prompt = ">";
		}

		#endregion

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			_gameServer = new GameServer();
			_timer = new System.Timers.Timer();
			_timer.Enabled = false;
			_timer.Interval = 1000;
			_timer.Elapsed += (sender, e) => {
				//System.Diagnostics.Debug.WriteLine("Polling " + DateTime.Now.ToString());
				var data = _gameServer.Poll();

				BeginInvokeOnMainThread(() => {

					var copy = data;

					var gameData = new GameData();
					gameData.Append(copy);

					try {
						var parser = IoC.Get<GameParser>();
						var result = parser.Parse(gameData);

						result.Apply(r => System.Diagnostics.Debug.WriteLine(r.GetType() + r.Matched + "\n\n"));

						var foundPrompt = result.OfType<PromptResult>().FirstOrDefault();
						if(foundPrompt != null)
						{
							_lastPrompt = foundPrompt;
						}

						Log(gameData.Current);
					}
					catch(Exception exc){
						Log("Parse Exception: " + exc.Message + "\n\n");
						Log(copy);
					}
				});
			};

			MainTextView.Editable = false;

			UsernameTextField.StringValue = "ilithi";

			LoginButton.Activated += (sender, e) =>
			{
				Log("Authenticating...\n\n");

				var token = Authenticate(UsernameTextField.StringValue, PasswordTextField.StringValue);
				if(token != null)
				{
					Log("\n\nAuthenticated...");
					Log("\nConnecting to game...\n");
					_gameServer.Connect(token);
					_timer.Enabled = true;
				}
			};

			LogoutButton.Activated += (sender, e) =>
			{
				_timer.Enabled = false;
				_gameServer.Close();
				Log("\n\nConnection closed.\n\n");
			};

			SubmitButton.Activated += (sender, e) =>
			{
				var command = CommandTextField.StringValue;
				var prompt = command;

				if(_lastPrompt != null)
				{
					prompt = _lastPrompt.Prompt + " " + prompt + "\n";
				}

				Log(prompt);

				CommandTextField.StringValue = string.Empty;
				_gameServer.SendCommand(command);
				_timer.Enabled = true;
			};
		}

		private ConnectionToken Authenticate(string account, string password)
		{
			using (var authServer = new AuthenticationServer("eaccess.play.net", 7900))
			{
				var authenticated = authServer.Authenticate(account, password);

				if (!authenticated) {
					Log("Authentication failed!");
					return null;
				}

				var gameList = authServer.GetGameList();
				gameList
					.Select(g => g.Code + ", " + g.Name)
					.Apply(Log);

				var characters = authServer.GetCharacterList("DR").ToList();
				characters
					.Select(c => c.CharacterId + ", " + c.Name)
					.Apply(Log);

				var token = authServer.ChooseCharacter(characters[0].CharacterId);
				return token;
			}
		}

		private void Log(string text)
		{
			MainTextView.TextStorage.Append(new NSAttributedString(text));

			var start = MainTextView.TextStorage.Value.Length - 2;
			start = start > -1 ? start : 0;
			var length = start >= 2 ? 2 : 0;
			MainTextView.ScrollRangeToVisible(new NSRange(start, length));
		}

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Pathfinder.Core;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Xml;
using Pathfinder.Core.Text;

namespace Pathfinder.Mac.Beta
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		private SimpleContainer _container;

		private GameServer _gameServer;
		private NewGameParser _gameParser;
		private System.Timers.Timer _timer;

		private PromptTag _lastPrompt;

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

			_container.PerRequest<NewGameParser>();

			_lastPrompt = new PromptTag();
			_lastPrompt.Prompt = ">";
		}

		#endregion

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			_gameServer = new GameServer();
			_gameParser = IoC.Get<NewGameParser>();

			_timer = new System.Timers.Timer();
			_timer.Enabled = false;
			_timer.Interval = 100;
			_timer.Elapsed += (sender, e) => {
				var data = _gameServer.Poll();

				BeginInvokeOnMainThread(() => {

					var copy = data;

					try {
						var result = _gameParser.Parse(Chunk.For(copy));

						//result.Tags.Apply(r => System.Diagnostics.Debug.WriteLine(string.Format("{0}::{1}\n\n", r.GetType(), r.Text)));

						var foundPrompt = result.Tags.OfType<PromptTag>().FirstOrDefault();
						if(foundPrompt != null)
						{
							_lastPrompt = foundPrompt;
						}

						result.Tags.OfType<RoomNameTag>().Apply(t => Log(t.Name));

						if(result.Chunk != null
							&& !string.IsNullOrWhiteSpace(result.Chunk.Text)
							&& !string.IsNullOrWhiteSpace(result.Chunk.Text.Trim()))
						{
							Log(result.Chunk.Text.Trim());

							if(_lastPrompt != null)
							{
								Log("\n" + _lastPrompt.Prompt + "\n");
							}
						}
					}
					catch(Exception exc){
						Log("Parse Exception: " + exc.Message + "\n\n");
						Log(copy);
					}
				});
			};

			MainTextView.Editable = false;

			LoginButton.Activated += (sender, e) =>
			{
				Log("Authenticating...\n\n");

				var account = UsernameTextField.StringValue;
				var password = PasswordTextField.StringValue;
				var character = CharacterTextField.StringValue;

				if(string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(character))
				{
					Log("Please enter all information\n\n");
					return;
				}

				var token = Authenticate(account, password, character);
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
				CommandTextField.StringValue = string.Empty;
				SendCommand(command);
			};
		}

		private void SendCommand(string command)
		{
			var prompt = command;

			if(_lastPrompt != null)
			{
				prompt = _lastPrompt.Prompt + " " + prompt + "\n";
			}

			Log(prompt);

			_gameServer.SendCommand(command);
			_timer.Enabled = true;
		}

		private ConnectionToken Authenticate(string account, string password, string character)
		{
			using (var authServer = new AuthenticationServer())
			{
				authServer.Connect("eaccess.play.net", 7900);
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

				var characterId = characters
					.Where(x => x.Name.ToLower() == character.ToLower())
					.Select(x => x.CharacterId)
					.FirstOrDefault();

				ConnectionToken token = null;
				if (!string.IsNullOrWhiteSpace(characterId)) {
					token = authServer.ChooseCharacter(characterId);
				}
				authServer.Close();
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

		public override void KeyUp(NSEvent theEvent)
		{
			base.KeyUp(theEvent);

			var keys = KeyUtil.GetKeys(theEvent);
			if(keys == NSKey.Return && !string.IsNullOrWhiteSpace(CommandTextField.StringValue)) {
				var command = CommandTextField.StringValue;
				CommandTextField.StringValue = string.Empty;
				SendCommand(command);
			}
		}

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
	}

}


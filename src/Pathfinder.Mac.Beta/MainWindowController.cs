using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Pathfinder.Core;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;
using Pathfinder.Core.Client;

namespace Pathfinder.Mac.Beta
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		private Bootstrapper _bootStrapper;
		private IGameServer _gameServer;

		private DateTime _roundTimeEnd;
		private Timer _timer;

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
			_bootStrapper = new Bootstrapper();

			_timer = new Timer();
			_timer.Interval = 1000;
			_timer.Elapsed += (sender, e) =>
			{
				var diff = _roundTimeEnd - DateTime.Now;
				SetRoundtime(diff.Seconds);
			};
		}

		#endregion

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			_gameServer = _bootStrapper.Build();

			IoC.Get<IGameState>().Set(ComponentKeys.RoomName, "[Derelict Road, Darkling Wood]");

			_gameServer.GameState.TextLog = (msg) => {
				BeginInvokeOnMainThread(() => {
					Log(msg);

					BeginInvokeOnMainThread(() => {
						LeftHandLabel.StringValue = string.Format("Left: {0}", _gameServer.GameState.Get(ComponentKeys.LeftHand));
						RightHandLabel.StringValue = string.Format("Right: {0}", _gameServer.GameState.Get(ComponentKeys.RightHand));
					});
				});
			};
			_gameServer.GameState.Roundtime = (rt) => {
				_roundTimeEnd = rt.RoundTime;
				var diff = _roundTimeEnd - DateTime.Now;
				SetRoundtime(diff.Seconds);
				_timer.Start();
			};

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

				var token = _gameServer.Authenticate("DR", account, password, character);
				if(token != null)
				{
					Log("\n\nAuthenticated...");
					Log("\nConnecting to game...\n");
					_gameServer.Connect(token);
				}
			};

			LogoutButton.Activated += (sender, e) =>
			{
				//_gameServer.Disconnect();
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
			var prompt = _gameServer.GameState.Get(ComponentKeys.Prompt) + " " + command + "\n";

			Log(prompt);

			_gameServer.SendCommand(command);
		}

		private void Log(string text)
		{
			var highlights = IoC.Get<Highlights>();
			highlights.For(text).Apply(Append);
		}

		private void Append(TextTag tag)
		{
			var defaultSettings = new DefaultSettings();

			var defaultColor = IoC.Get<HighlightSettings>().Get(HighlightKeys.Default).Color;

			var color = !string.IsNullOrWhiteSpace(tag.Color) ? tag.Color : defaultColor;
			var font = tag.Mono ? defaultSettings.MonoFont : defaultSettings.Font;

			MainTextView.TextStorage.Append(tag.Text.CreateString(color.ToNSColor(), font));

			var start = MainTextView.TextStorage.Value.Length - 2;
			start = start > -1 ? start : 0;
			var length = start >= 2 ? 2 : 0;
			MainTextView.ScrollRangeToVisible(new NSRange(start, length));
		}

		private void SetRoundtime(int count)
		{
			if(count < 0)
			{
				count = 0;
				_timer.Stop();
			}

			BeginInvokeOnMainThread(() => RoundtimeLabel.StringValue = string.Format("RT: {0}", count));
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


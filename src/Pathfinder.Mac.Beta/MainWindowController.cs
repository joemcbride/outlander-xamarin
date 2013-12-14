using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Pathfinder.Core;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Client;
using Pathfinder.Core.Text;

namespace Pathfinder.Mac.Beta
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		private Bootstrapper _bootStrapper;
		private IGameServer _gameServer;
		private CommandCache _commandCache;

		private DateTime _roundTimeEnd;
		private Timer _timer;

		private ExpTracker _expTracker = new ExpTracker();

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
			_commandCache = new CommandCache();

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

			var notifyLogger = new NotificationLogger();
			notifyLogger.OnError = (err) => {
				BeginInvokeOnMainThread(()=> {
					LogSystem("\n" + err.Message + "\n\n");
				});
			};

			var compositeLogger = IoC.Get<ILog>().As<CompositeLog>();
			compositeLogger.Add(notifyLogger);

			_gameServer.GameState.Exp = (exp) => {

				_expTracker.Update(exp);

				var tag = TextTag.For(_expTracker.StringDisplay(), "#ffffff", true);

				BeginInvokeOnMainThread(()=>{
					ReplaceText(tag, ExpTextView);
				});
			};

			_gameServer.GameState.TextLog = (msg) => {
				BeginInvokeOnMainThread(() => {
					Log(msg);

					LeftHandLabel.StringValue = string.Format("Left: {0}", _gameServer.GameState.Get(ComponentKeys.LeftHand));
					RightHandLabel.StringValue = string.Format("Right: {0}", _gameServer.GameState.Get(ComponentKeys.RightHand));
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
				var account = UsernameTextField.StringValue;
				var password = PasswordTextField.StringValue;
				var character = CharacterTextField.StringValue;

				if(string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(character))
				{
					LogSystem("Please enter all information\n");
					return;
				}

				LogSystem("\nAuthenticating...\n");

				var token = _gameServer.Authenticate("DR", account, password, character);
				if(token != null)
				{
					LogSystem("Authenticated...\n");
					_gameServer.Connect(token);
				}
				else
				{
					LogSystem("Unable to authenticate.\n");
				}
			};

			LogoutButton.Activated += (sender, e) =>
			{
				//_gameServer.Disconnect();
				LogSystem("\n\nConnection closed.\n\n");
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

			_commandCache.Add(command);
			_gameServer.SendCommand(command);
		}

		private void Log(string text)
		{
			var highlights = IoC.Get<Highlights>();
			highlights.For(text).Apply(t => Append(t, MainTextView));
		}

		private void LogSystem(string text)
		{
			LogSystem(text, "#ffbb00");
		}

		private void LogSystem(string text, string color)
		{
			Append(TextTag.For(text, color, true), MainTextView);
		}

		private void Append(TextTag tag, NSTextView textView)
		{
			var defaultSettings = new DefaultSettings();

			var defaultColor = IoC.Get<HighlightSettings>().Get(HighlightKeys.Default).Color;

			var color = !string.IsNullOrWhiteSpace(tag.Color) ? tag.Color : defaultColor;
			var font = tag.Mono ? defaultSettings.MonoFont : defaultSettings.Font;

			textView.TextStorage.BeginEditing();
			textView.TextStorage.Append(tag.Text.CreateString(color.ToNSColor(), font));
			textView.TextStorage.EndEditing();

			var start = textView.TextStorage.Value.Length - 2;
			start = start > -1 ? start : 0;
			var length = start >= 2 ? 2 : 0;
			textView.ScrollRangeToVisible(new NSRange(start, length));
		}

		private void ReplaceText(TextTag tag, NSTextView textView)
		{
			var defaultSettings = new DefaultSettings();

			var defaultColor = IoC.Get<HighlightSettings>().Get(HighlightKeys.Default).Color;

			var color = !string.IsNullOrWhiteSpace(tag.Color) ? tag.Color : defaultColor;
			var font = tag.Mono ? defaultSettings.MonoFont : defaultSettings.Font;

			textView.TextStorage.BeginEditing();
			textView.TextStorage.SetString(tag.Text.CreateString(color.ToNSColor(), font));
			textView.TextStorage.EndEditing();
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

			if(keys == NSKey.UpArrow)
			{
				_commandCache.MovePrevious();
				CommandTextField.StringValue = _commandCache.Current;
			}

			if(keys == NSKey.DownArrow)
			{
				_commandCache.MoveNext();
				CommandTextField.StringValue = _commandCache.Current;
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

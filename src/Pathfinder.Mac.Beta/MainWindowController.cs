using System;
using System.Collections.Generic;
using System.IO;
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
		private IServiceLocator _services;

		private Bootstrapper _bootStrapper;
		private IGameServer _gameServer;
		private CommandCache _commandCache;

		private ICommandProcessor _commandProcessor;
		private IScriptLog _scriptLog;

		private ExpTracker _expTracker;
		private HighlightSettings _highlightSettings;

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
			_expTracker = new ExpTracker();
		}

		#endregion

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			Window.Title = "Outlander";

			_gameServer = _bootStrapper.Build();
			_services = _bootStrapper.ServiceLocator();

			var appSettings = _services.Get<AppSettings>();
			var homeDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents/Outlander");
			appSettings.HomeDirectory = homeDir;

			new BuildAppDirectories().Build(appSettings);

			_commandProcessor = _services.Get<ICommandProcessor>();
			_scriptLog = _services.Get<IScriptLog>();
			_services.Get<IRoundtimeHandler>().Changed += (sender, e) => {
				SetRoundtime(e);
			};

			_scriptLog.Info += (sender, e) => {
				var log = "[{0}]: ({1}) {2}\n".ToFormat(e.Name, e.LineNumber, e.Data);

				BeginInvokeOnMainThread(()=> {

					var hasLineFeed = MainTextView.TextStorage.Value.EndsWith("\n");

					if(!hasLineFeed)
						log = "\n" + log;

					var tag = TextTag.For(log, "ADFF2F");

					Append(tag, MainTextView);
				});
			};

			_scriptLog.NotifyStarted += (sender, e) => {
				var log = "[{0}]: {1} - script started\n".ToFormat(e.Name, e.Started.ToString("G"));

				var tag = TextTag.For(log, "ADFF2F");

				BeginInvokeOnMainThread(()=> {
					Append(tag, MainTextView);
				});
			};

			_scriptLog.NotifyAborted += (sender, e) => {
				var log = "[{0}]: total runtime {1} seconds\n".ToFormat(e.Name, Math.Round(e.Runtime.TotalSeconds, 2));

				var tag = TextTag.For(log, "ADFF2F");

				BeginInvokeOnMainThread(()=> {
					Append(tag, MainTextView);
				});
			};

			_highlightSettings = _services.Get<HighlightSettings>();

			var notifyLogger = new NotificationLogger();
			notifyLogger.OnError = (err) => {
				BeginInvokeOnMainThread(()=> {
					LogSystem("\n" + err.Message + "\n\n");
				});
			};

			var compositeLogger = _services.Get<ILog>().As<CompositeLog>();
			compositeLogger.Add(notifyLogger);

			_gameServer.GameState.Tags = (tags) => {
				tags.Apply(t => {
					t.As<AppTag>().IfNotNull(appInfo => {
						BeginInvokeOnMainThread(() => {
							Window.Title = string.Format("{0}: {1} - {2}", appInfo.Game, appInfo.Character, "Outlander");
						});
					});

					t.As<StreamTag>().IfNotNull(streamTag => {
						if(!string.IsNullOrWhiteSpace(streamTag.Id) && streamTag.Id.Equals("logons")) {
							var text = TextTag.For(string.Format("[{0}]{1}", DateTime.Now.ToString("HH:mm"), streamTag.Value), string.Empty, true);

							BeginInvokeOnMainThread(()=> {
								Append(text, ArrivalsTextView);
							});
						}
					});

					t.As<SpellTag>().IfNotNull(s => {
						BeginInvokeOnMainThread(()=>{
							SpellLabel.StringValue = string.Format("S: {0}", s.Spell);
						});
					});

					t.As<VitalsTag>().IfNotNull(v => {
						UpdateVitals();
					});

					var builder = new StringBuilder();
					_gameServer.GameState.Get(ComponentKeys.RoomTitle).IfNotNullOrEmpty(s=>builder.AppendLine(s));
					_gameServer.GameState.Get(ComponentKeys.RoomDescription).IfNotNullOrEmpty(s=>builder.AppendLine(s));
					_gameServer.GameState.Get(ComponentKeys.RoomObjects).IfNotNullOrEmpty(s=>builder.AppendLine(s));
					_gameServer.GameState.Get(ComponentKeys.RoomPlayers).IfNotNullOrEmpty(s=>builder.AppendLine(s));
					_gameServer.GameState.Get(ComponentKeys.RoomExists).IfNotNullOrEmpty(s=>builder.AppendLine(s));

					BeginInvokeOnMainThread(()=> {
						LogRoom(builder.ToString(), RoomTextView);
					});
				});
			};

			_gameServer.GameState.Exp = (exp) => {

				_expTracker.Update(exp);

				var skills = _expTracker.SkillsWithExp().ToList();
				var tags = skills
						.OrderBy(x => x.Name)
						.Select(x =>
						{
							var color = x.IsNew ? _highlightSettings.Get(HighlightKeys.Whisper).Color : string.Empty;
							return TextTag.For(x.Display() + "\n", color, true);
						});

				BeginInvokeOnMainThread(()=>{
					ReplaceText(tags, ExpTextView);
				});
			};

			_gameServer.GameState.TextLog += (msg) => {
				BeginInvokeOnMainThread(() => {
					Log(msg, MainTextView);

					LeftHandLabel.StringValue = string.Format("L: {0}", _gameServer.GameState.Get(ComponentKeys.LeftHand));
					RightHandLabel.StringValue = string.Format("R: {0}", _gameServer.GameState.Get(ComponentKeys.RightHand));
				});
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
				LogSystem("\n\nConnection closed.\n\n");
			};

			SubmitButton.Activated += (sender, e) =>
			{
				var command = CommandTextField.StringValue;
				CommandTextField.StringValue = string.Empty;
				SendCommand(command);
			};
		}

		private void UpdateVitals()
		{
			BeginInvokeOnMainThread(()=>{

				HealthTextField.StringValue = string.Format("Health {0}%", _gameServer.GameState.Get(ComponentKeys.Health));
				ManaTextField.StringValue = string.Format("Mana {0}%", _gameServer.GameState.Get(ComponentKeys.Mana));
				StaminaTextField.StringValue = string.Format("Stamina {0}%", _gameServer.GameState.Get(ComponentKeys.Stamina));
				ConcentrationTextField.StringValue = string.Format("Conc {0}%", _gameServer.GameState.Get(ComponentKeys.Concentration));
				SpiritTextField.StringValue = string.Format("Spirit {0}%", _gameServer.GameState.Get(ComponentKeys.Spirit));
			});
		}

		private void SendCommand(string command)
		{
			_commandCache.Add(command);

			var replaced = _commandProcessor.Eval(command);

			var prompt = _gameServer.GameState.Get(ComponentKeys.Prompt) + " " + replaced + "\n";

			var hasLineFeed = MainTextView.TextStorage.Value.EndsWith("\n");

			if(!hasLineFeed)
				prompt = "\n" + prompt;

			Log(prompt, MainTextView);

			_commandProcessor.Process(replaced);
		}

		private void LogRoom(string text, NSTextView textView)
		{
			var defaultSettings = new DefaultSettings();
			var defaultColor = _highlightSettings.Get(HighlightKeys.Default).Color;

			textView.TextStorage.BeginEditing();
			textView.TextStorage.SetString("".CreateString(defaultColor.ToNSColor(), defaultSettings.Font));
			textView.TextStorage.EndEditing();

			var highlights = _services.Get<Highlights>();
			highlights.For(text).Apply(t => Append(t, textView));
		}

		private void Log(string text, NSTextView textView)
		{
			var highlights = _services.Get<Highlights>();
			highlights.For(text).Apply(t => Append(t, textView));
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
			var text = tag.Text.Replace("&lt;", "<").Replace("&gt;", ">");

			var defaultSettings = new DefaultSettings();

			var defaultColor = _highlightSettings.Get(HighlightKeys.Default).Color;

			var color = !string.IsNullOrWhiteSpace(tag.Color) ? tag.Color : defaultColor;
			var font = tag.Mono ? defaultSettings.MonoFont : defaultSettings.Font;

			textView.TextStorage.BeginEditing();
			textView.TextStorage.Append(text.CreateString(color.ToNSColor(), font));
			textView.TextStorage.EndEditing();

			var start = textView.TextStorage.Value.Length - 2;
			start = start > -1 ? start : 0;
			var length = start >= 2 ? 2 : 0;
			textView.ScrollRangeToVisible(new NSRange(start, length));
		}

		private void ReplaceText(IEnumerable<TextTag> tags, NSTextView textView)
		{
			var defaultSettings = new DefaultSettings();
			var defaultColor = _highlightSettings.Get(HighlightKeys.Default).Color;

			textView.TextStorage.BeginEditing();
			textView.TextStorage.SetString("".CreateString(defaultColor.ToNSColor()));
			tags.Apply(tag => {
				var color = !string.IsNullOrWhiteSpace(tag.Color) ? tag.Color : defaultColor;
				var font = tag.Mono ? defaultSettings.MonoFont : defaultSettings.Font;
				textView.TextStorage.Append(tag.Text.CreateString(color.ToNSColor(), font));
			});
			textView.TextStorage.EndEditing();
		}

		private void SetRoundtime(long count)
		{
			if(count < 0)
			{
				count = 0;
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

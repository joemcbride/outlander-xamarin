using System;
using System.Collections.Generic;
using System.Drawing;
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

		private Timer _spellTimer;
		private string _spell;
		private int _count;

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

			_spellTimer = new Timer();
			_spellTimer.Interval = 1000;
			_spellTimer.Elapsed += (sender, e) => {
				_count++;
				BeginInvokeOnMainThread(()=>SpellLabel.StringValue = "S: ({0}) {1}".ToFormat(_count, _spell));
			};
		}

		#endregion

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			HealthLabel.BackgroundColor = "#800000";
			ManaLabel.Label = "Mana";
			ConcentrationLabel.BackgroundColor = "#009999";
			ConcentrationLabel.Label = "Concentration";
			ConcentrationLabel.TextOffset = new PointF(55, 1);
			StaminaLabel.BackgroundColor = "#004000";
			StaminaLabel.Label = "Stamina";
			SpiritLabel.BackgroundColor = "#400040";
			SpiritLabel.Label = "Spirit";

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
				string log;
				if(e.LineNumber > -1)
				{
					log = "[{0}({1})]:  {2}\n".ToFormat(e.Name, e.LineNumber, e.Data);
				}
				else
				{
					log = "[{0}]:  {1}\n".ToFormat(e.Name, e.Data);
				}

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

				BeginInvokeOnMainThread(()=> {

					var hasLineFeed = MainTextView.TextStorage.Value.EndsWith("\n");

					if(!hasLineFeed)
						log = "\n" + log;

					var tag = TextTag.For(log, "ADFF2F");

					Append(tag, MainTextView);
				});
			};

			_scriptLog.NotifyAborted += (sender, e) => {
				var log = "[{0}]: total runtime {1} seconds\n".ToFormat(e.Name, Math.Round(e.Runtime.TotalSeconds, 2));

				BeginInvokeOnMainThread(()=> {

					var hasLineFeed = MainTextView.TextStorage.Value.EndsWith("\n");

					if(!hasLineFeed)
						log = "\n" + log;

					var tag = TextTag.For(log, "ADFF2F");

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
						BeginInvokeOnMainThread(() => {
							_spell = s.Spell;
							_count = 0;
							SpellLabel.StringValue = string.Format("S: {0}", s.Spell);

							if(!string.Equals(_spell, "None"))
							{
								_spellTimer.Start();
							}
							else
							{
								_spellTimer.Stop();
							}
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
						}).ToList();
				var now = DateTime.Now;
				tags.Add(TextTag.For("Last updated: {0:hh\\:mm\\:ss tt}\n".ToFormat(now), string.Empty, true));
				if(_expTracker.StartedTracking.HasValue)
					tags.Add(TextTag.For("Tracking for: {0:hh\\:mm\\:ss}\n".ToFormat(now - _expTracker.StartedTracking.Value), string.Empty, true));

				BeginInvokeOnMainThread(()=> {
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
				var game = GameTextField.StringValue;
				var account = UsernameTextField.StringValue;
				var password = PasswordTextField.StringValue;
				var character = CharacterTextField.StringValue;

				if(string.IsNullOrWhiteSpace(game) || string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(character))
				{
					LogSystem("Please enter all information\n");
					return;
				}

				LogSystem("\nAuthenticating...\n");

				var token = _gameServer.Authenticate(game, account, password, character);
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
		}

		private void UpdateVitals()
		{
			BeginInvokeOnMainThread(()=>{
				HealthLabel.Value = _gameServer.GameState.Get(ComponentKeys.Health).AsFloat();
				ManaLabel.Value = _gameServer.GameState.Get(ComponentKeys.Mana).AsFloat();
				StaminaLabel.Value = _gameServer.GameState.Get(ComponentKeys.Stamina).AsFloat();
				ConcentrationLabel.Value = _gameServer.GameState.Get(ComponentKeys.Concentration).AsFloat();
				SpiritLabel.Value = _gameServer.GameState.Get(ComponentKeys.Spirit).AsFloat();
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

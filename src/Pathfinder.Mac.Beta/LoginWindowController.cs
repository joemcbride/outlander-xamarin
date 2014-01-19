using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Linq;
using Outlander.Core;
using Outlander.Core.Client;

namespace Outlander.Mac.Beta
{
	public partial class LoginWindowController : MonoMac.AppKit.NSWindowController
	{
		private IServiceLocator _services;
		private Action<ConnectModel> _complete;
		private Action _cancel;

		#region Constructors

		// Called when created from unmanaged code
		public LoginWindowController(IntPtr handle)
			: base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public LoginWindowController(NSCoder coder)
			: base(coder)
		{
			Initialize();
		}
		// Call to load from the XIB/NIB file
		public LoginWindowController()
			: base("LoginWindow")
		{
			Initialize();
		}
		// Shared initialization code
		private void Initialize()
		{
		}

		#endregion

		//strongly typed window accessor
		public new LoginWindow Window {
			get {
				return (LoginWindow)base.Window;
			}
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			LoginButton.Activated += (sender, e) => {
				HideSheet();
				var model = BindingSource.SelectedObjects.First().As<ConnectModel>();
				var settings = _services.Get<AppSettings>();
				_services.Get<IProfileLoader>().Save(
					new Profile
					{
						Name = settings.Profile,
						Account = model.Account,
						Game = model.Game,
						Character = model.Character
					}
				);
				_complete(model);
			};
			CancelButton.Activated += (sender, e) => {
				HideSheet();
				_cancel();
			};
		}

		public void ShowSheet(NSWindow newParent, ConnectModel model, IServiceLocator services, Action<ConnectModel> complete, Action cancel)
		{
			_services = services;
			_complete = complete;
			_cancel = cancel;
			Window.BeginSheet(newParent);
			BindingSource.AddObject(model);
		}

		public void HideSheet()
		{
			Window.EndSheet(this);
		}
	}

	[Register("ConnectModel")]
	public class ConnectModel : NSObject
	{
		[Export("Account")]
		public string Account {get;set;}

		[Export("Password")]
		public string Password {get;set;}

		[Export("Game")]
		public string Game {get;set;}

		[Export("Character")]
		public string Character {get;set;}

		public static ConnectModel For(string account, string game, string character)
		{
			return new ConnectModel { Account = account, Game = game, Character = character };
		}
	}
}

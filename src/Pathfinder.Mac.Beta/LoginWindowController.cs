using System;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Outlander.Mac.Beta
{
	public partial class LoginWindowController : MonoMac.AppKit.NSWindowController
	{
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
			};
		}

		public void ShowSheet(NSWindow newParent)
		{
			Window.BeginSheet(newParent);
		}

		public void HideSheet()
		{
			Window.EndSheet(this);
		}
	}
}

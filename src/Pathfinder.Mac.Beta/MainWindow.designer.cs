// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Pathfinder.Mac.Beta
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField CommandTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton LoginButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton LogoutButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView MainTextView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSecureTextField PasswordTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton SubmitButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField UsernameTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (UsernameTextField != null) {
				UsernameTextField.Dispose ();
				UsernameTextField = null;
			}

			if (PasswordTextField != null) {
				PasswordTextField.Dispose ();
				PasswordTextField = null;
			}

			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}

			if (LogoutButton != null) {
				LogoutButton.Dispose ();
				LogoutButton = null;
			}

			if (MainTextView != null) {
				MainTextView.Dispose ();
				MainTextView = null;
			}

			if (CommandTextField != null) {
				CommandTextField.Dispose ();
				CommandTextField = null;
			}

			if (SubmitButton != null) {
				SubmitButton.Dispose ();
				SubmitButton = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}

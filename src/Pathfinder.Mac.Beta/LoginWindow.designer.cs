// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Outlander.Mac.Beta
{
	[Register ("LoginWindowController")]
	partial class LoginWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton LoginButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}
		}
	}

	[Register ("LoginWindow")]
	partial class LoginWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}

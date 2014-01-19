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
	[Register ("NewProfileWindowController")]
	partial class NewProfileWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField MessageLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField NameTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton OKButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NameTextField != null) {
				NameTextField.Dispose ();
				NameTextField = null;
			}

			if (OKButton != null) {
				OKButton.Dispose ();
				OKButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (MessageLabel != null) {
				MessageLabel.Dispose ();
				MessageLabel = null;
			}
		}
	}

	[Register ("NewProfileWindow")]
	partial class NewProfileWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}

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
	[Register ("ProfileSelectorController")]
	partial class ProfileSelectorController
	{
		[Outlet]
		MonoMac.AppKit.NSSegmentedControl AddRemoveControl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton CloseButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSArrayController Profiles { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView ProfileTableView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Profiles != null) {
				Profiles.Dispose ();
				Profiles = null;
			}

			if (ProfileTableView != null) {
				ProfileTableView.Dispose ();
				ProfileTableView = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (AddRemoveControl != null) {
				AddRemoveControl.Dispose ();
				AddRemoveControl = null;
			}
		}
	}

	[Register ("ProfileSelector")]
	partial class ProfileSelector
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}

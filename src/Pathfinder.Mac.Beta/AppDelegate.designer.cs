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
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoMac.AppKit.NSMenuItem Connect { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem CutMenuItem { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem NewMenuItem { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem SwitchProfile { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CutMenuItem != null) {
				CutMenuItem.Dispose ();
				CutMenuItem = null;
			}

			if (NewMenuItem != null) {
				NewMenuItem.Dispose ();
				NewMenuItem = null;
			}

			if (SwitchProfile != null) {
				SwitchProfile.Dispose ();
				SwitchProfile = null;
			}

			if (Connect != null) {
				Connect.Dispose ();
				Connect = null;
			}
		}
	}
}

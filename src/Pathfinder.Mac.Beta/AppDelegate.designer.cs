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
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoMac.AppKit.NSMenuItem CutMenuItem { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem NewMenuItem { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NewMenuItem != null) {
				NewMenuItem.Dispose ();
				NewMenuItem = null;
			}

			if (CutMenuItem != null) {
				CutMenuItem.Dispose ();
				CutMenuItem = null;
			}
		}
	}
}

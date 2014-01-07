using System;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Outlander.Mac.Beta
{
	public static class WindowExtensions
	{
		public static void BeginSheet(this NSWindow window, NSWindow parent)
		{
			NSApplication.SharedApplication.BeginSheet(window, parent);
		}

		public static void EndSheet(this NSWindow window, NSObject sender = null)
		{
			NSApplication.SharedApplication.EndSheet(window);
			window.OrderOut(sender);
			window.Close();
		}
	}
}

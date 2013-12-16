using System;
using System.Collections.Generic;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Pathfinder.Mac.Beta
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		private List<MainWindowController> _windows = new List<MainWindowController>();

		public AppDelegate()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
			var mainWindowController = new MainWindowController();
			mainWindowController.Window.MakeKeyAndOrderFront(this);
			_windows.Add(mainWindowController);
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			NewMenuItem.Activated += (sender, e) => {
				var mainWindowController = new MainWindowController();
				mainWindowController.Window.MakeKeyAndOrderFront(this);
				_windows.Add(mainWindowController);
			};
		}
	}
}

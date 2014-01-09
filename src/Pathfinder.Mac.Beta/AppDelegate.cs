using System;
using System.Collections.Generic;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Outlander.Mac.Beta;

namespace Outlander.Mac.Beta
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		private List<MainWindowController> _windows = new List<MainWindowController>();
		//private LoginWindowController _settings;

		public AppDelegate()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
			LaunchNewWindow();

//			var controller = new SettingsWindowController();
//			controller.Window.MakeKeyAndOrderFront(this);

//			_settings = new LoginWindowController();
//			_settings.ShowSheet(_windows[0].Window);
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			NewMenuItem.Activated += (sender, e) => {
				LaunchNewWindow();
			};
		}

		private void LaunchNewWindow()
		{
			var mainWindowController = new MainWindowController();
			mainWindowController.Window.MakeKeyAndOrderFront(this);
			_windows.Add(mainWindowController);
		}
	}
}

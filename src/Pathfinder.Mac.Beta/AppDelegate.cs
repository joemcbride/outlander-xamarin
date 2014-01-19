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

		[Export("Profiles")]
		public NSMutableArray Profiles { get; set; }

		public AppDelegate()
		{
			Profiles = new NSMutableArray();
		}

		public override void DidBecomeActive(NSNotification notification)
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
			LaunchNewWindow();
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			NewMenuItem.Activated += (sender, e) => {
				LaunchNewWindow();
			};
			SwitchProfile.Activated += (sender, e) => {
				ActiveController.SwitchProfile();
			};
			Connect.Activated += (sender, e) => {
				ActiveController.Connect();
			};
		}

		private void LaunchNewWindow()
		{
			var mainWindowController = new MainWindowController();
			mainWindowController.Window.DidBecomeKey += (sender, e) => {
				ActiveController = mainWindowController;
			};
			mainWindowController.Window.MakeKeyAndOrderFront(this);
			_windows.Add(mainWindowController);
		}

		private MainWindowController ActiveController {
			get;
			set;
		}
	}

	public interface IScreen
	{
		NSWindow Window { get; }
	}

	public class Conductor<T> where T : IScreen
	{
	}
}

using System;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Outlander.Mac.Beta
{
	public partial class SettingsWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SettingsWindowController(IntPtr handle)
			: base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public SettingsWindowController(NSCoder coder)
			: base(coder)
		{
			Initialize();
		}
		// Call to load from the XIB/NIB file
		public SettingsWindowController()
			: base("SettingsWindow")
		{
			Initialize();
		}
		// Shared initialization code
		private void Initialize()
		{
		}

		#endregion

		//strongly typed window accessor
		public new SettingsWindow Window {
			get {
				return (SettingsWindow)base.Window;
			}
		}

		public void ShowModal(NSWindow newParent)
		{
			//NSApplication.SharedApplication.BeginSheet(Window, newParent);
		}
	}
}

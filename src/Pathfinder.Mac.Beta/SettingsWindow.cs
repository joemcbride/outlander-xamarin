using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Outlander.Mac.Beta
{
	public partial class SettingsWindow : NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public SettingsWindow(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public SettingsWindow(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		// Shared initialization code
		void Initialize()
		{
			BackgroundColor = "#4A4A4A".ToNSColor();
		}

		#endregion
	}
}

using System;
using MonoMac.Foundation;

namespace Outlander.Mac.Beta
{
	public partial class LoginWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public LoginWindow(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public LoginWindow(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		// Shared initialization code
		void Initialize()
		{
		}

		#endregion
	}
}

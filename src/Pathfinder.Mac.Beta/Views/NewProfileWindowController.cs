using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Outlander.Core;
using System.IO;
using Outlander.Core.Client;

namespace Outlander.Mac.Beta
{
	public partial class NewProfileWindowController : MonoMac.AppKit.NSWindowController
	{
		private IServiceLocator _services;
		private Action<string> _complete;
		private Action _cancel;

		#region Constructors

		// Called when created from unmanaged code
		public NewProfileWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public NewProfileWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		// Call to load from the XIB/NIB file
		public NewProfileWindowController() : base("NewProfileWindow")
		{
			Initialize();
		}
		// Shared initialization code
		void Initialize()
		{
		}

		#endregion

		//strongly typed window accessor
		public new NewProfileWindow Window {
			get {
				return (NewProfileWindow)base.Window;
			}
		}

		public void Init(IServiceLocator services, Action<string> complete, Action cancel)
		{
			_services = services;
			_complete = complete;
			_cancel = cancel;
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			OKButton.Activated += (sender, e) => {
				var profileLoader = _services.Get<IProfileLoader>();
				var name = NameTextField.StringValue;
				if(profileLoader.Exists(name)) {
					MessageLabel.StringValue = "A profile with that name already exists.";
					return;
				}

				Window.Close();

				_complete(NameTextField.StringValue);
			};

			CancelButton.Activated += (sender, e) => {
				Window.Close();
				_cancel();
			};
		}
	}
}

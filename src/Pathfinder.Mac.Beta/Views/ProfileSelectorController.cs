using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Outlander.Core.Client;
using Outlander.Core;

namespace Outlander.Mac.Beta
{
	public partial class ProfileSelectorController : MonoMac.AppKit.NSWindowController
	{
		private IProfileLoader _profileLoader;
		private IAppSettingsLoader _appSettingsLoader;
		private AppSettings _appSettings;
		private Action _complete;
		private Action _cancel;

		#region Constructors

		// Called when created from unmanaged code
		public ProfileSelectorController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public ProfileSelectorController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		// Call to load from the XIB/NIB file
		public ProfileSelectorController() : base("ProfileSelector")
		{
			Initialize();
		}
		// Shared initialization code
		void Initialize()
		{
		}

		#endregion

		//strongly typed window accessor
		public new ProfileSelector Window {
			get {
				return (ProfileSelector)base.Window;
			}
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			Window.WillClose += (sender, e) => {
				SaveAndClose();
			};
			CloseButton.Activated += (sender, e) => {
				Window.Close();
			};
			AddRemoveControl.Activated += (sender, e) => {
				var add = AddRemoveControl.SelectedSegment == 1;
			};
		}

		public void InitWithProfiles(
			IEnumerable<Profile> profiles,
			AppSettings settings,
			IProfileLoader profileLoader,
			IAppSettingsLoader appSettingsLoader,
			Action complete,
			Action cancel)
		{
			_appSettings = settings;
			_profileLoader = profileLoader;
			_appSettingsLoader = appSettingsLoader;
			_complete = complete;
			_cancel = cancel;

			Profiles.Content.As<NSMutableArray>().RemoveAllObjects();

			int idx = -1;
			profiles.Apply((p, i) => {
				if(string.Equals(settings.Profile, p.Name)){
					idx = i;
				}
				Profiles.AddObject(ProfileInfo.For(p));
			});

			Profiles.SelectionIndex = idx;
		}

		private void SaveAndClose()
		{
			var selectedProfile = Profiles.SelectedObjects.First().As<ProfileInfo>();
			_appSettings.Profile = selectedProfile.Profile;

			_appSettingsLoader.SaveConfig();

			Profiles
				.ArrangedObjects()
				.Select(x => x.As<ProfileInfo>())
				.Apply(x => {
					_profileLoader.SaveProfile(Profile.For(x.Profile, x.Account, x.Game, x.Character));
				});

			_complete();
		}
	}

	[Register("ProfileInfo")]
	public class ProfileInfo : NSObject
	{
		[Export("Profile")]
		public string Profile {get;set;}

		[Export("Game")]
		public string Game {get;set;}

		[Export("Account")]
		public string Account {get;set;}

		[Export("Character")]
		public string Character {get;set;}

		public static ProfileInfo For(Profile profile)
		{
			return For(profile.Name, profile.Game, profile.Account, profile.Character);
		}

		public static ProfileInfo For(string profile, string game, string account, string character)
		{
			return new ProfileInfo { Profile = profile, Game = game, Account = account, Character = character };
		}
	}

	[Register("ProfileDataSource")]
	public class ProfileDataSource : NSTableViewDataSource
	{
		private List<ProfileInfo> _profiles;

		public ProfileDataSource()
			: this(new List<ProfileInfo>())
		{
		}

		public ProfileDataSource(List<ProfileInfo> profiles)
		{
			_profiles = profiles;
		}

		public override int GetRowCount(NSTableView tableView)
		{
			return _profiles.Count;
		}

		public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
		{
//			var valueKey = (string)(NSString)tableColumn.Identifier;
//			var dataRow = _profiles[row];

			return null;
		}

		public override void SetObjectValue(NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row)
		{
//			var valueKey = (string)(NSString)tableColumn.Identifier;
//			var dataRow = _profiles[row];
		}
	}
}


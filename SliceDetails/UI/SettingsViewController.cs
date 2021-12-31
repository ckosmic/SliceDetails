using BeatSaberMarkupLanguage.Attributes;

namespace SliceDetails.UI
{
	internal class SettingsViewController : PersistentSingleton<SettingsViewController>
	{
		[UIValue("show-pause")]
		public bool ShowInPauseMenu {
			get { return Plugin.Settings.ShowInPauseMenu; }
			set { Plugin.Settings.ShowInPauseMenu = value; }
		}

		[UIValue("show-completion")]
		public bool ShowInCompletionScreen
		{
			get { return Plugin.Settings.ShowInCompletionScreen; }
			set { Plugin.Settings.ShowInCompletionScreen = value; }
		}

		[UIValue("show-handles")]
		public bool ShowHandle
		{
			get { return Plugin.Settings.ShowHandle; }
			set { Plugin.Settings.ShowHandle = value; }
		}

		[UIValue("true-offsets")]
		public bool TrueCutOffsets
		{
			get { return Plugin.Settings.TrueCutOffsets; }
			set { Plugin.Settings.TrueCutOffsets = value; }
		}
	}
}

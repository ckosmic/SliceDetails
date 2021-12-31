using SiraUtil.Affinity;
using SliceDetails.UI;

namespace SliceDetails.AffinityPatches
{
	internal class PauseMenuManagerPatches : IAffinity
	{
		private readonly PauseUIController _pauseUIController;

		public PauseMenuManagerPatches(PauseUIController pauseUIController) {
			_pauseUIController = pauseUIController;
		}

		[AffinityPostfix]
		[AffinityPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.ShowMenu))]
		internal void ShowMenuPostfix(ref PauseMenuManager __instance) { 
			if (Plugin.Settings.ShowInPauseMenu && _pauseUIController != null)
				_pauseUIController.PauseMenuOpened(__instance);
		}

		[AffinityPostfix]
		[AffinityPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.StartResumeAnimation))]
		internal void StartResumeAnimationPostfix(ref PauseMenuManager __instance) {
			if (Plugin.Settings.ShowInPauseMenu && _pauseUIController != null)
				_pauseUIController.PauseMenuClosed(__instance);
		}
	}
}

using SiraUtil.Affinity;
using SliceDetails.UI;

namespace SliceDetails.AffinityPatches
{
	internal class MenuTransitionsHelperPatch : IAffinity
	{
		private readonly PauseUIController _pauseUIController;

		public MenuTransitionsHelperPatch(PauseUIController pauseUIController) {
			_pauseUIController = pauseUIController;
		}

		[AffinityPostfix]
		[AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMainGameSceneDidFinish))]
		internal void Postfix(ref MenuTransitionsHelper __instance) {
			if (Plugin.Settings.ShowInPauseMenu)
				_pauseUIController.CleanUp();
		}
	}
}

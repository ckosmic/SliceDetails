using SiraUtil.Affinity;
using SliceDetails.UI;

namespace SliceDetails.AffinityPatches
{
	internal class ResultsViewControllerPatch : IAffinity
	{
		private readonly UICreator _uiCreator;

		public ResultsViewControllerPatch(UICreator uiCreator) {
			_uiCreator = uiCreator;
		}

		[AffinityPostfix]
		[AffinityPatch(typeof(ResultsViewController), nameof(ResultsViewController.DisableResultEnvironmentController))]
		internal void Postfix(ref ResultsViewController __instance) {
			if(Plugin.Settings.ShowInCompletionScreen)
				_uiCreator.RemoveFloatingScreen();
		}
	}
}

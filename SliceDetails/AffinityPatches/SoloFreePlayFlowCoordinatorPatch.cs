using SiraUtil.Affinity;
using SliceDetails.UI;
using UnityEngine;

namespace SliceDetails.AffinityPatches
{
	internal class SoloFreePlayFlowCoordinatorPatch : IAffinity
	{
		private readonly UICreator _uiCreator;

		public SoloFreePlayFlowCoordinatorPatch(UICreator uiCreator) {
			_uiCreator = uiCreator;
		}

		[AffinityPostfix]
		[AffinityPatch(typeof(SoloFreePlayFlowCoordinator), "ProcessLevelCompletionResultsAfterLevelDidFinish")]
		internal void Postfix(ref SoloFreePlayFlowCoordinator __instance, LevelCompletionResults levelCompletionResults) {
			if (levelCompletionResults.levelEndAction == LevelCompletionResults.LevelEndAction.None && Plugin.Settings.ShowInCompletionScreen) {
				_uiCreator.CreateFloatingScreen(Plugin.Settings.ResultsUIPosition, Quaternion.Euler(Plugin.Settings.ResultsUIRotation));
			}
		}
	}
}

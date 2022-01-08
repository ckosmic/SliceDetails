using SiraUtil.Affinity;
using SliceDetails.UI;
using UnityEngine;

namespace SliceDetails.AffinityPatches
{
	internal class PartyFreePlayFlowCoordinatorPatch : IAffinity
	{
		private readonly UICreator _uiCreator;

		public PartyFreePlayFlowCoordinatorPatch(UICreator uiCreator)
		{
			_uiCreator = uiCreator;
		}

		[AffinityPostfix]
		[AffinityPatch(typeof(PartyFreePlayFlowCoordinator), "ProcessLevelCompletionResultsAfterLevelDidFinish")]
		internal void Postfix(ref PartyFreePlayFlowCoordinator __instance, LevelCompletionResults levelCompletionResults)
		{
			if (levelCompletionResults.levelEndAction == LevelCompletionResults.LevelEndAction.None && Plugin.Settings.ShowInCompletionScreen)
			{
				_uiCreator.CreateFloatingScreen(Plugin.Settings.ResultsUIPosition, Quaternion.Euler(Plugin.Settings.ResultsUIRotation));
			}
		}
	}
}
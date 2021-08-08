using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SliceDetails.HarmonyPatches
{
	[HarmonyPatch(typeof(SoloFreePlayFlowCoordinator), "ProcessLevelCompletionResultsAfterLevelDidFinish")]
	class SoloFreePlayFlowCoordinatorPatch
	{
		static void Postfix(ref SoloFreePlayFlowCoordinator __instance, LevelCompletionResults levelCompletionResults) {
			if (levelCompletionResults.levelEndAction == LevelCompletionResults.LevelEndAction.None && Plugin.Settings.ShowInCompletionScreen) {
				UICreator.instance.Create(Plugin.Settings.ResultsUIPosition, Quaternion.Euler(Plugin.Settings.ResultsUIRotation));
			}
		}
	}
}

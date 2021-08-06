using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SliceDetails.HarmonyPatches
{
	[HarmonyPatch(typeof(MenuTransitionsHelper), "HandleMainGameSceneDidFinish")]
	class MenuTransitionsHelperPatch
	{
		static void Postfix(ref MenuTransitionsHelper __instance) {
			PauseUIController.instance.CleanUp();
		}
	}
}

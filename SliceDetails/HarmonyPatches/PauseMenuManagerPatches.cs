using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SliceDetails.HarmonyPatches
{
	[HarmonyPatch(typeof(PauseMenuManager), "ShowMenu")]
	class PauseMenuManagerPatches_ShowMenu
	{
		static void Postfix(ref PauseMenuManager __instance) {
			if(Plugin.Settings.ShowInPauseMenu)
				PauseUIController.instance.PauseMenuOpened(__instance);
		}
	}

	[HarmonyPatch(typeof(PauseMenuManager), "StartResumeAnimation")]
	class PauseMenuManagerPatches_StartResumeAnimation
	{
		static void Postfix(ref PauseMenuManager __instance) {
			if(Plugin.Settings.ShowInPauseMenu)
				PauseUIController.instance.PauseMenuClosed(__instance);
		}
	}
}

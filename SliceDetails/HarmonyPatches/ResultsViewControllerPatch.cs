using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SliceDetails.HarmonyPatches
{
	[HarmonyPatch(typeof(ResultsViewController), "DisableResultEnvironmentController")]
	class ResultsViewControllerPatch
	{
		static void Postfix(ref ResultsViewController __instance) {
			UICreator.instance.Remove();
		}
	}
}

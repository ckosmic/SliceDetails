using IPA.Config.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SliceDetails.Settings
{
	internal class SettingsStore
	{
		public Vector3 ResultsUIPosition = new Vector3(-3.25f, 3.25f, 1.75f);
		public Vector3 ResultsUIRotation = new Vector3(340.0f, 292.0f, 0.0f);
		public Vector3 PauseUIPosition = new Vector3(-3.0f, 1.5f, 0.0f);
		public Vector3 PauseUIRotation = new Vector3(0.0f, 270.0f, 0.0f);
		public bool ShowInPauseMenu = true;
		public bool ShowInCompletionScreen = true;
		public bool ShowHandle = false;
		public bool TrueCutOffsets = true;
	}
}

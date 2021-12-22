using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;
using SliceDetails.Installers;
using HarmonyLib;
using SliceDetails.Settings;
using BeatSaberMarkupLanguage.Settings;

namespace SliceDetails
{

	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }
		internal static SettingsStore Settings { get; private set; }

		private Harmony _harmony;

		[Init]
		public void Init(IPALogger logger, Config config, Zenjector zenject) {
			Instance = this;
			Log = logger;
			Settings = config.Generated<SettingsStore>();
			BSMLSettings.instance.AddSettingsMenu("SliceDetails", $"SliceDetails.UI.Views.settingsView.bsml", SettingsViewController.instance);
			zenject.Install<SDGameInstaller>(Location.StandardPlayer); // For Sira 3
			//zenject.OnGame<SDGameInstaller>(false).OnlyForStandard(); // For Sira 2
		}

		[OnStart]
		public void OnApplicationStart() {
			_harmony = new Harmony("SliceDetails");
			_harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());


			new GameObject("SDSliceProcessor").AddComponent<SliceProcessor>();
			new GameObject("SDCompletionUICreator").AddComponent<UICreator>();
		}
	}
}

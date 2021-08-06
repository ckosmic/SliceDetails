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
		/// <summary>
		/// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
		/// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
		/// Only use [Init] with one Constructor.
		/// </summary>
		public void Init(IPALogger logger, Config config, Zenjector zenject) {
			Instance = this;
			Log = logger;
			Settings = config.Generated<SettingsStore>();
			zenject.OnGame<SDGameInstaller>(false).OnlyForStandard();
			Log.Info("SliceDetails initialized.");
		}

		[OnStart]
		public void OnApplicationStart() {
			Log.Debug("OnApplicationStart");

			_harmony = new Harmony("SliceDetails");
			_harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());


			new GameObject("SDSliceProcessor").AddComponent<SliceProcessor>();
			new GameObject("SDCompletionUICreator").AddComponent<UICreator>();
		}

		[OnExit]
		public void OnApplicationQuit() {
			Log.Debug("OnApplicationQuit");

		}
	}
}

using BeatSaberMarkupLanguage.FloatingScreen;
using HMUI;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SliceDetails.UI
{
	internal class UICreator
	{
		private readonly GridViewController _gridViewController;
		private readonly SliceProcessor _sliceProcessor;
		private readonly SiraLog _siraLog;

		private FloatingScreen _floatingScreen;

		public HoverHintController hoverHintController;

		public UICreator(SiraLog siraLog, GridViewController gridViewController, SliceProcessor sliceProcesssor) {
			_siraLog = siraLog;
			_gridViewController = gridViewController;
			_sliceProcessor = sliceProcesssor;
		}

		public void CreateFloatingScreen(Vector3 position, Quaternion rotation) {
			_siraLog.Info("Creating floating screen: " + (_gridViewController == null));
			_gridViewController.UpdateUINotesHoverHintController();


			_floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(150f, 120f), true, position, rotation);
			_floatingScreen.SetRootViewController(_gridViewController, ViewController.AnimationType.None);
			_floatingScreen.ShowHandle = Plugin.Settings.ShowHandle;
			_floatingScreen.HandleSide = FloatingScreen.Side.Bottom;
			_floatingScreen.HighlightHandle = true;
			_floatingScreen.handle.transform.localScale = Vector3.one * 5.0f;
			_floatingScreen.handle.transform.localPosition = new Vector3(0.0f, -25.0f, 0.0f);
			_floatingScreen.HandleReleased += OnHandleReleased;
			_floatingScreen.gameObject.name = "SliceDetailsScreen";
			_floatingScreen.transform.localScale = Vector3.one * 0.03f;

			_gridViewController.SetTileScores();
			_gridViewController.transform.localScale = Vector3.one;
			_gridViewController.transform.localEulerAngles = Vector3.zero;
			_gridViewController.gameObject.SetActive(true);
		}

		public void RemoveFloatingScreen() {
			// Destroying the hover hint panel breaks everything so move it out of the screen before destroying
			if (_floatingScreen != null) {
				if (_floatingScreen.transform.GetComponentInChildren<HoverHintPanel>(true)) {
					Transform hoverHintPanel = _floatingScreen.transform.GetComponentInChildren<HoverHintPanel>(true).transform;
					hoverHintPanel.SetParent(null);
				}
				_gridViewController.transform.SetParent(null);
				_gridViewController.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(_floatingScreen.gameObject);
			}
			_sliceProcessor.ResetProcessor();
		}

		public void ParentFloatingScreen(Transform parent) {
			_floatingScreen.transform.SetParent(parent);
		}

		private void OnHandleReleased(object sender, FloatingScreenHandleEventArgs args) {
			if (SceneManager.GetActiveScene().name == "MainMenu") {
				Plugin.Settings.ResultsUIPosition = _floatingScreen.transform.position;
				Plugin.Settings.ResultsUIRotation = _floatingScreen.transform.eulerAngles;
			} else if (SceneManager.GetActiveScene().name == "GameCore") {
				Plugin.Settings.PauseUIPosition = _floatingScreen.transform.position;
				Plugin.Settings.PauseUIRotation = _floatingScreen.transform.eulerAngles;
			}
		}
	}
}

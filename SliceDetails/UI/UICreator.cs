using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using HMUI;
using SliceDetails.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUIControls;

namespace SliceDetails
{
	internal class UICreator : MonoBehaviour
	{
		public static UICreator instance { get; private set; }

		public static Sprite spr_arrow = ResourceUtilities.LoadSpriteFromResource("SliceDetails.Resources.arrow.png");
		public static Sprite spr_dot = ResourceUtilities.LoadSpriteFromResource("SliceDetails.Resources.dot.png");
		public static Sprite spr_RoundRect10;

		public HoverHintController hoverHintController;

		private HoverHintController _hoverHintController;
		private GridViewController _gridViewController;
		private FloatingScreen _floatingScreen;
		private PauseMenuManager _pauseMenuManager;

		private void Start() {
			if (instance != null) Destroy(instance.gameObject);
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			StartCoroutine(DelayedGetHoverHintController());
			StartCoroutine(DelayedGetPauseMenuManager(delegate {
				spr_RoundRect10 = _pauseMenuManager.transform.Find("Wrapper/MenuWrapper/Canvas/MainBar/LevelBarSimple/BG").GetComponent<ImageView>().sprite;
			}));
			
		}

		public GridViewController Create(Vector3 position, Quaternion rotation) {
			if (_floatingScreen != null || instance._gridViewController != null)
				Remove();

			instance._gridViewController = BeatSaberUI.CreateViewController<GridViewController>();

			_floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(150f, 120f), true, position, rotation);
			_floatingScreen.SetRootViewController(instance._gridViewController, ViewController.AnimationType.None);
			_floatingScreen.ShowHandle = Plugin.Settings.ShowHandle;
			_floatingScreen.HandleSide = FloatingScreen.Side.Bottom;
			_floatingScreen.handle.transform.localScale = Vector3.one * 5.0f;
			_floatingScreen.handle.transform.localPosition = new Vector3(0.0f, -25.0f, 0.0f);
			_floatingScreen.HandleReleased += OnHandleReleased;
			_floatingScreen.gameObject.name = "SliceDetailsScreen";
			if (SceneManager.GetActiveScene().name == "GameCore") {
				StartCoroutine(DelayedGetPauseMenuManager(delegate {
					// Screen mover needs to be completely remade for dragging to work in-game
					VRPointer gameVRPointer = _pauseMenuManager.transform.Find("Wrapper/MenuWrapper/EventSystem").GetComponent<VRPointer>();
					Destroy(_floatingScreen.screenMover);
					_floatingScreen.screenMover = gameVRPointer.gameObject.AddComponent<FloatingScreenMoverPointer>();
					_floatingScreen.screenMover.Init(_floatingScreen, gameVRPointer);
				}));
			}

			return instance._gridViewController;
		}

		public void Remove() {
			// Destroying the hover hint panel breaks everything so move it out of the screen before destroying
			if (_floatingScreen != null) {
				if (_floatingScreen.transform.GetComponentInChildren<HoverHintPanel>(true)) {
					Transform hoverHintPanel = _floatingScreen.transform.GetComponentInChildren<HoverHintPanel>(true).transform;
					hoverHintPanel.SetParent(null);
				}
				Destroy(_floatingScreen.gameObject);
			}
			SliceProcessor.instance.ResetProcessor();
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

		public void CreateHoverHintControllerInstance() {
			hoverHintController = Instantiate(_hoverHintController);
		}

		public void RevertHoverHintControllerInstance() {
			DestroyImmediate(hoverHintController.gameObject);
			hoverHintController = _hoverHintController;
		}

		IEnumerator DelayedGetHoverHintController() {
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<HoverHintController>().Any());
			_hoverHintController = Resources.FindObjectsOfTypeAll<HoverHintController>().First();
			hoverHintController = _hoverHintController;
		}

		IEnumerator DelayedGetPauseMenuManager(Action callback) {
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<PauseMenuManager>().Any());
			_pauseMenuManager = Resources.FindObjectsOfTypeAll<PauseMenuManager>().First();
			callback.Invoke();
		}
	}
}

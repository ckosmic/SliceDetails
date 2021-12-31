using UnityEngine;
using Zenject;

namespace SliceDetails.UI
{
	internal class PauseUIController : IInitializable
	{
		private readonly SliceRecorder _sliceRecorder;
		private readonly UICreator _uiCreator;
		private readonly HoverHintControllerHandler _hoverHintControllerHandler;

		private GridViewController _gridViewController;
		private GameObject _gridViewControllerParent;

		public PauseUIController(SliceRecorder sliceRecorder, UICreator uiCreator, GridViewController gridViewController, HoverHintControllerHandler hoverHintControllerHandler) {
			_sliceRecorder = sliceRecorder;
			_uiCreator = uiCreator;
			_gridViewController = gridViewController;
			_hoverHintControllerHandler = hoverHintControllerHandler;
		}

		public void Initialize() {
			if (Plugin.Settings.ShowInPauseMenu) {
				_hoverHintControllerHandler.CloneHoverHintController();
				_uiCreator.CreateFloatingScreen(Plugin.Settings.PauseUIPosition, Quaternion.Euler(Plugin.Settings.PauseUIRotation));
				_gridViewControllerParent = _gridViewController.transform.parent.gameObject;
				_gridViewControllerParent?.SetActive(false);
			}
		}

		public void PauseMenuOpened(PauseMenuManager pauseMenuManager) {
			_gridViewControllerParent?.SetActive(true);
			_uiCreator.ParentFloatingScreen(pauseMenuManager.transform);
			_sliceRecorder.ProcessSlices();
			_gridViewController.SetTileScores();
		}

		public void PauseMenuClosed(PauseMenuManager pauseMenuManager) {
			_gridViewController.CloseModal(false);
			_gridViewControllerParent?.SetActive(false);
		}

		public void CleanUp() {
			_gridViewController.CloseModal(false);
			_uiCreator.RemoveFloatingScreen();
		}
	}
}

using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRUIControls;
using Zenject;

namespace SliceDetails
{
	internal class PauseUIController : IInitializable, IDisposable
	{
		public static PauseUIController instance { get; private set; }

		private static GridViewController _gridViewController;

		public void Initialize() {
			instance = this;
			if (Plugin.Settings.ShowInPauseMenu) {
				_gridViewController = UICreator.instance.Create(Plugin.Settings.PauseUIPosition, Quaternion.Euler(Plugin.Settings.PauseUIRotation));
				_gridViewController.transform.parent.gameObject.SetActive(false);
				UICreator.instance.CreateHoverHintControllerInstance();
			}
		}

		public void PauseMenuOpened(PauseMenuManager pauseMenuManager) {
			_gridViewController.transform.parent.gameObject.SetActive(true);
			UICreator.instance.ParentFloatingScreen(pauseMenuManager.transform);
			SliceRecorder.instance.ProcessSlices();
			_gridViewController.SetTileScores();
		}

		public void PauseMenuClosed(PauseMenuManager pauseMenuManager) {
			_gridViewController.CloseModal(false);
			_gridViewController.transform.parent.gameObject.SetActive(false);
		}

		public void CleanUp() {
			_gridViewController.CloseModal(false);
			UICreator.instance.Remove();
			UICreator.instance.RevertHoverHintControllerInstance();
		}

		public void Dispose() {
			
		}
	}
}

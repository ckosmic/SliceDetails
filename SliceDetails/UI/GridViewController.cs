using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities;
using SiraUtil.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using SliceDetails.Data;

namespace SliceDetails.UI
{
	public enum OrderedNoteCutDirection
	{
		UpLeft = 0,
		Up = 1,
		UpRight = 2,
		Left = 3,
		Any = 4,
		Right = 5,
		DownLeft = 6,
		Down = 7,
		DownRight = 8,
		None = 9
	}

	[HotReload(RelativePathToLayout = @"Views\gridView.bsml")]
	[ViewDefinition("SliceDetails.UI.Views.gridView.bsml")]
	internal class GridViewController : BSMLAutomaticViewController {

		private SiraLog _siraLog;
		private AssetLoader _assetLoader;
		private HoverHintControllerHandler _hoverHintControllerHandler;
		private SliceProcessor _sliceProcessor;

		[UIObject("tile-grid")]
		private readonly GameObject _tileGrid;
		[UIObject("tile-row")]
		private readonly GameObject _tileRow;
		[UIComponent("tile")]
		private readonly ClickableImage _tile;

		[UIObject("note-modal")]
		private readonly GameObject _noteModal;
		[UIObject("note-horizontal")]
		private readonly GameObject _noteHorizontal;
		[UIObject("note-grid")]
		private GameObject _noteGrid;
		[UIObject("note-row")]
		private GameObject _noteRow;

		[UIComponent("note")]
		private readonly ImageView _note;
		[UIComponent("note-dir-arrow")]
		private readonly ImageView _noteDirArrow;
		[UIComponent("note-cut-arrow")]
		private readonly ImageView _noteCutArrow;
		[UIComponent("note-cut-distance")]
		private readonly ImageView _noteCutDistance;
		[UIComponent("sd-version")]
		private readonly TextMeshProUGUI _sdVersionText;

		private List<ClickableImage> _tiles = new List<ClickableImage>();
		private List<NoteUI> _notes = new List<NoteUI>();
		private SelectedTileIndicator _selectedTileIndicator;
		private BasicUIAudioManager _basicUIAudioManager;


		[Inject]
		internal void Construct(SiraLog siraLog, AssetLoader assetLoader, HoverHintControllerHandler hoverHintControllerHandler, SliceProcessor sliceProcessor) {
			_siraLog = siraLog;
			_assetLoader = assetLoader;
			_hoverHintControllerHandler = hoverHintControllerHandler;
			_sliceProcessor = sliceProcessor;
			_siraLog.Debug("GridViewController Constructed");
		}

		[UIAction("#post-parse")]
		public void PostParse() {
			_noteDirArrow.gameObject.name = "NoteDirArrow";
			_noteCutArrow.gameObject.name = "NoteCutArrow";
			_noteCutDistance.gameObject.name = "NoteCutDistance";

			_sdVersionText.text = $"SliceDetails v{ System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3) }";
			ReflectionUtil.InvokeMethod<object, TextMeshProUGUI>(_sdVersionText, "Awake"); // For some reason this is necessary
			_sdVersionText.rectTransform.sizeDelta = new Vector2(40.0f, 10.0f);
			_sdVersionText.transform.localPosition = new Vector3(0.0f, -17.0f, 0.0f);

			_tiles = new List<ClickableImage>();
			// Create first row of tiles
			for (int i = 0; i < 4; i++) {
				ClickableImage tileInstance = Instantiate(_tile.gameObject, _tileRow.transform).GetComponent<ClickableImage>();
				_tiles.Add(tileInstance);
			}

			// Create other 2 rows of tiles
			for (int i = 0; i < 2; i++) {
				GameObject tileRowInstance = Instantiate(_tileRow, _tileGrid.transform);
				tileRowInstance.transform.SetAsFirstSibling();
				_tiles.AddRange(tileRowInstance.GetComponentsInChildren<ClickableImage>());
			}

			// Set tile click events and data
			for (int i = 0; i < _tiles.Count; i++) {
				_tiles[i].OnClickEvent = _tile.OnClickEvent;
				_tiles[i].OnClickEvent += SetNotesData;
				_tiles[i].DefaultColor = _tile.DefaultColor;
				_tiles[i].HighlightColor = _tile.HighlightColor;
			}

			Transform noteParent = _noteRow.transform;
			Transform rowParent = _noteGrid.transform;
			_notes = new List<NoteUI>();
			HoverHintController currentHoverHintController = _hoverHintControllerHandler.hoverHintController;
			for (int i = 0; i < 18; i++) {
				if (i % 9 == 0) {
					rowParent = Instantiate(_noteGrid, _noteHorizontal.transform).transform;
				}
				if (i % 3 == 0) {
					noteParent = Instantiate(_noteRow, rowParent).transform;
				}

				ColorType colorType = (ColorType)(i >= 9 ? 1 : 0);
				OrderedNoteCutDirection cutDirection = (OrderedNoteCutDirection)(i % 9);
				NoteUI uiNote = Instantiate(_note.gameObject, noteParent).AddComponent<NoteUI>();
				uiNote.Initialize(cutDirection, colorType, _assetLoader);
				uiNote.SetHoverHintController(currentHoverHintController);

				_notes.Add(uiNote);
			}

			_selectedTileIndicator = new GameObject("SelectedTileIndicator").AddComponent<SelectedTileIndicator>();
			_selectedTileIndicator.Initialize(_assetLoader);
			_selectedTileIndicator.transform.SetParent(_noteModal.transform, false);
			_selectedTileIndicator.transform.localPosition = new Vector3(0f, 30f, 0f);

			_basicUIAudioManager = Resources.FindObjectsOfTypeAll<BasicUIAudioManager>().First(x => x.GetComponent<AudioSource>().enabled && x.isActiveAndEnabled);

			DestroyImmediate(_note.gameObject);
			DestroyImmediate(_noteRow);
			DestroyImmediate(_noteGrid);
			DestroyImmediate(_tile.gameObject);
		}

		public void SetTileScores() {
			for (int i = 0; i < _tiles.Count; i++) {
				FormattableText text = _tiles[i].transform.GetComponentInChildren<FormattableText>();
				if (_sliceProcessor.tiles[i].atLeastOneNote)
					text.text = String.Format("{0:0.00}", _sliceProcessor.tiles[i].scoreAverage);
				else
					text.text = "";
			}
		}

		private void SetNotesData(PointerEventData eventData) {
			int tileIndex = _tiles.IndexOf(eventData.pointerPress.GetComponent<ClickableImage>());
			_selectedTileIndicator.SetSelectedTile(tileIndex);
			Tile tile = _sliceProcessor.tiles[tileIndex];
			for (int i = 0; i < _notes.Count; i++) {
				float angle = tile.angleAverages[i];
				float offset = tile.offsetAverages[i];
				Score score = tile.scoreAverages[i];

				_notes[i].SetNoteData(angle, offset, score);
			}
		}

		[UIAction("#presentNotesModal")]
		public void PresentModal() {
			if (_basicUIAudioManager != null)
				_basicUIAudioManager.HandleButtonClickEvent();
		}

		public void CloseModal(bool animated) {
			_noteModal.GetComponent<ModalView>().Hide(animated);
		}

		public void UpdateUINotesHoverHintController() {
			HoverHintController currentHoverHintController = _hoverHintControllerHandler.hoverHintController;
			for (int i = 0; i < _notes.Count; i++) {
				_notes[i].SetHoverHintController(currentHoverHintController);
			}
		}
	}
}

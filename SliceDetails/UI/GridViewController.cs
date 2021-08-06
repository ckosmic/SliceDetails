using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SliceDetails
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

	internal class GridViewController : BSMLResourceViewController {
		// For this method of setting the ResourceName, this class must be the first class in the file.
		public override string ResourceName => "SliceDetails.UI.Views.gridView.bsml";

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

		private List<ClickableImage> _tiles = new List<ClickableImage>();
		private List<NoteUI> _notes = new List<NoteUI>();
		private SelectedTileIndicator _selectedTileIndicator;

		private const float _scale = 0.03f;

		[UIAction("#post-parse")]
		public void PostParse() {
			transform.parent.localScale = Vector3.one * _scale;

			_noteDirArrow.gameObject.name = "NoteDirArrow";
			_noteCutArrow.gameObject.name = "NoteCutArrow";
			_noteCutDistance.gameObject.name = "NoteCutDistance";

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
				uiNote.Initialize(cutDirection, colorType);

				_notes.Add(uiNote);
			}

			SetTileScores();

			_selectedTileIndicator = new GameObject("SelectedTileIndicator").AddComponent<SelectedTileIndicator>();
			_selectedTileIndicator.Initialize();
			_selectedTileIndicator.transform.SetParent(_noteModal.transform, false);
			_selectedTileIndicator.transform.localPosition = new Vector3(0f, 30f, 0f);

			DestroyImmediate(_note.gameObject);
			DestroyImmediate(_noteRow);
			DestroyImmediate(_noteGrid);
			DestroyImmediate(_tile.gameObject);
		}

		public void SetTileScores() {
			for (int i = 0; i < _tiles.Count; i++) {
				FormattableText text = _tiles[i].transform.GetComponentInChildren<FormattableText>();
				if (SliceProcessor.instance.tiles[i].atLeastOneNote)
					text.text = String.Format("{0:0.00}", SliceProcessor.instance.tiles[i].scoreAverage);
				else
					text.text = "";
			}
		}

		private void SetNotesData(PointerEventData eventData) {
			int tileIndex = _tiles.IndexOf(eventData.pointerPress.GetComponent<ClickableImage>());
			_selectedTileIndicator.SetSelectedTile(tileIndex);
			Tile tile = SliceProcessor.instance.tiles[tileIndex];
			for (int i = 0; i < _notes.Count; i++) {
				float angle = tile.angleAverages[i];
				float offset = tile.offsetAverages[i];
				Score score = tile.scoreAverages[i];

				_notes[i].SetNoteData(angle, offset, score);
			}
		}

		public void CloseModal(bool animated) {
			_noteModal.GetComponent<ModalView>().Hide(animated);
		}
	}
}

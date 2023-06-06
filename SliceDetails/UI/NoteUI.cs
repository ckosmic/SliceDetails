using HMUI;
using IPA.Utilities;
using SliceDetails.Utils;
using SliceDetails.Data;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SliceDetails.UI
{

	internal class NoteUI : MonoBehaviour
	{
		private ImageView _directionArrowImage;
		private Transform _cutGroup;
		private ImageView _cutArrowImage;
		private ImageView _cutDistanceImage;
		private ImageView _backgroundImage;

		private Color _noteColor;
		private HoverHint _noteHoverHint;
		private HoverHintController _hoverHintController;
		private TextMeshProUGUI _hoverPanelTmpro;

		private float _noteRotation;

		public void Initialize(OrderedNoteCutDirection cutDirection, ColorType colorType, AssetLoader assetLoader) {
			transform.localScale = Vector3.one * 0.9f;

			_backgroundImage = GetComponent<ImageView>();
			_directionArrowImage = transform.Find("NoteDirArrow").GetComponent<ImageView>();
			_cutArrowImage = transform.Find("NoteCutArrow").GetComponent<ImageView>();
			_cutDistanceImage = transform.Find("NoteCutDistance").GetComponent<ImageView>();

			_cutGroup = new GameObject("NoteCutGroup").transform;
			_cutGroup.SetParent(_backgroundImage.transform);
			_cutGroup.localPosition = Vector3.zero;
			_cutGroup.localScale = Vector3.one;
			_cutGroup.localRotation = Quaternion.identity;
			_cutArrowImage.transform.SetParent(_cutGroup);
			_cutDistanceImage.transform.SetParent(_cutGroup);

			if (colorType == ColorType.ColorA)
				_noteColor = ColorSchemeManager.GetMainColorScheme().saberAColor;
			else if (colorType == ColorType.ColorB)
				_noteColor = ColorSchemeManager.GetMainColorScheme().saberBColor;

			_backgroundImage.color = _noteColor;
			_cutDistanceImage.color = new Color(0.0f, 1.0f, 0.0f, 0.75f);

			Texture2D square = new Texture2D(2, 2);
			square.filterMode = FilterMode.Point;
			square.Apply();
			_cutDistanceImage.sprite = Sprite.Create(square, new Rect(0, 0, square.width, square.height), new Vector2(0, 0), 100);

			_noteHoverHint = _backgroundImage.gameObject.AddComponent<HoverHint>();
			_noteHoverHint.text = "";


			switch (cutDirection) {
				case OrderedNoteCutDirection.Down:
					_noteRotation = 0.0f;
					break;
				case OrderedNoteCutDirection.Up:
					_noteRotation = 180.0f;
					break;
				case OrderedNoteCutDirection.Left:
					_noteRotation = 270.0f;
					break;
				case OrderedNoteCutDirection.Right:
					_noteRotation = 90.0f;
					break;
				case OrderedNoteCutDirection.DownLeft:
					_noteRotation = 315.0f;
					break;
				case OrderedNoteCutDirection.DownRight:
					_noteRotation = 45.0f;
					break;
				case OrderedNoteCutDirection.UpLeft:
					_noteRotation = 225.0f;
					break;
				case OrderedNoteCutDirection.UpRight:
					_noteRotation = 135.0f;
					break;
				case OrderedNoteCutDirection.Any:
					_noteRotation = 0.0f;
					_directionArrowImage.sprite = assetLoader.spr_dot;
					break;
			}

			transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, _noteRotation));
		}

		public void SetHoverHintController(HoverHintController hoverHintController) {
			_hoverHintController = hoverHintController;
			HoverHintPanel hhp = _hoverHintController.GetField<HoverHintPanel, HoverHintController>("_hoverHintPanel");
			// Skew cringe skew cringe
			hhp.GetComponent<ImageView>().SetField("_skew", 0.0f);
			_hoverPanelTmpro = hhp.GetComponentInChildren<TextMeshProUGUI>();
			_hoverPanelTmpro.fontStyle = FontStyles.Normal;
			_hoverPanelTmpro.alignment = TextAlignmentOptions.Left;
			_hoverPanelTmpro.overflowMode = TextOverflowModes.Overflow;
			_hoverPanelTmpro.enableWordWrapping = false;
			ContentSizeFitter csf = _hoverPanelTmpro.gameObject.AddComponent<ContentSizeFitter>();
			csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		public void SetNoteData(float angle, float offset, float offsetDeviation, Score score, int count) {
			_noteHoverHint.SetField("_hoverHintController", _hoverHintController);

			if (angle == 0f && offset == 0f) {
				_backgroundImage.color = Color.gray;
				_cutArrowImage.gameObject.SetActive(false);
				_cutDistanceImage.gameObject.SetActive(false);
				_directionArrowImage.color = new Color(0.8f, 0.8f, 0.8f);
				_noteHoverHint.text = "";
			} else {
				_backgroundImage.color = _noteColor;
				_cutArrowImage.gameObject.SetActive(true);
				_cutDistanceImage.gameObject.SetActive(true);
				_cutGroup.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, angle - _noteRotation - 90f));
				if (Plugin.Settings.TrueCutOffsets) {
					_cutArrowImage.transform.localPosition = new Vector3(offset * 20.0f, 0f, 0f);
                    _cutDistanceImage.transform.localPosition = new Vector3((offset + offsetDeviation / 2.0f) * 20.0f, 0f, 0f);
                    _cutDistanceImage.transform.localScale = new Vector2(offsetDeviation * 1.25f, 1.0f);
                } else {
					_cutArrowImage.transform.localPosition = new Vector3(offset * (30.0f + score.Offset), 0f, 0f);
                    _cutDistanceImage.transform.localPosition = new Vector3((offset + offsetDeviation / 2.0f) * (30.0f + score.Offset), 0f, 0f);
                    _cutDistanceImage.transform.localScale = new Vector2(offsetDeviation * (1.875f + score.Offset * 0.0625f), 1.0f);
                }
				_directionArrowImage.color = Color.white;
				string noteNotes = count == 1 ? "note" : "notes";
				_noteHoverHint.text = "Average score (" + count + " " + noteNotes + ")\n<color=#ff0000>" + String.Format("{0:0.00}", score.TotalScore) + "</color>\n<color=#666666><size=3><line-height=115%>Pre-swing - " + String.Format("{0:0.00}", score.PreSwing) + "\nPost-swing - " + String.Format("{0:0.00}", score.PostSwing) + "\nAccuracy - " + String.Format("{0:0.00}", score.Offset) + "</line-height></size></color>";
			}
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using SliceDetails.Data;
using SiraUtil.Logging;

namespace SliceDetails
{
	internal class SliceRecorder : UnityEngine.Object, IInitializable, IDisposable
	{
		private readonly BeatmapObjectManager _beatmapObjectManager;
		private readonly SliceProcessor _sliceProcessor;
		private readonly ScoreController _scoreController;

		[Inject] private SiraLog _siraLog;

		private Dictionary<NoteData, NoteInfo> _noteSwingInfos = new Dictionary<NoteData, NoteInfo>();
		private List<NoteInfo> _noteInfos = new List<NoteInfo>();

		public SliceRecorder(BeatmapObjectManager beatmapObjectManager, ScoreController scoreController, SliceProcessor sliceProcessor) {
			_beatmapObjectManager = beatmapObjectManager;
			_scoreController = scoreController;
			_sliceProcessor = sliceProcessor;
		}

		public void Initialize() {
			_beatmapObjectManager.noteWasCutEvent += OnNoteWasCut;
			_scoreController.scoringForNoteFinishedEvent += ScoringForNoteFinishedHandler;
			_sliceProcessor.ResetProcessor();
		}

		public void Dispose() {
			_beatmapObjectManager.noteWasCutEvent -= OnNoteWasCut;
			_scoreController.scoringForNoteFinishedEvent -= ScoringForNoteFinishedHandler;
			// Process slices once the map ends
			ProcessSlices();
		}

		public void ClearSlices() {
			_noteInfos.Clear();
			ProcessSlices();
		}

		public void ProcessSlices() {
			_sliceProcessor.ProcessSlices(_noteInfos);
		}

		private void OnNoteWasCut(NoteController noteController, in NoteCutInfo noteCutInfo) {
			if (noteController.noteData.colorType == ColorType.None || !noteCutInfo.allIsOK) return;
			ProcessNote(noteController, noteCutInfo);
		}

		private void ProcessNote(NoteController noteController, NoteCutInfo noteCutInfo) {
			if (noteController == null) return;
			
			Vector2 noteGridPosition;
			noteGridPosition.y = (int)noteController.noteData.noteLineLayer;
			noteGridPosition.x = noteController.noteData.lineIndex;
			int noteIndex = (int)(noteGridPosition.y * 4 + noteGridPosition.x);

			// No ME notes allowed >:(
			if (noteGridPosition.x >= 4 || noteGridPosition.y >= 3 || noteGridPosition.x < 0 || noteGridPosition.y < 0) return;

			Vector2 cutDirection = new Vector3(-noteCutInfo.cutNormal.y, noteCutInfo.cutNormal.x);
			float cutAngle = Mathf.Atan2(cutDirection.y, cutDirection.x) * Mathf.Rad2Deg + 180f;

			float cutOffset = noteCutInfo.cutDistanceToCenter;
			Vector3 noteCenter = noteController.noteTransform.position;
			if (Vector3.Dot(noteCutInfo.cutNormal, noteCutInfo.cutPoint - noteCenter) > 0f)
			{
				cutOffset = -cutOffset;
			}

			NoteInfo noteInfo = new NoteInfo(noteController.noteData, noteCutInfo, cutAngle, cutOffset, noteGridPosition, noteIndex);

			if (!_noteSwingInfos.ContainsKey(noteController.noteData)) 
			{
				_noteSwingInfos.Add(noteController.noteData, noteInfo);
			}
		}

		public void ScoringForNoteFinishedHandler(ScoringElement scoringElement) {
			if (_noteSwingInfos.TryGetValue(scoringElement.noteData, out NoteInfo noteSwingInfo) && scoringElement is GoodCutScoringElement goodScoringElement)
			{
				IReadonlyCutScoreBuffer cutScoreBuffer = goodScoringElement.cutScoreBuffer;

				int preSwing = cutScoreBuffer.beforeCutScore;
				int postSwing = cutScoreBuffer.afterCutScore;
				int offset = cutScoreBuffer.centerDistanceCutScore;

				switch (goodScoringElement.noteData.scoringType)
				{
					case NoteData.ScoringType.Normal:
						noteSwingInfo.score = new Score(preSwing, postSwing, offset);
						_noteInfos.Add(noteSwingInfo);
						break;
					case NoteData.ScoringType.SliderHead:
						if (!Plugin.Settings.CountArcs) break;
						noteSwingInfo.score = new Score(preSwing, null, offset);
						_noteInfos.Add(noteSwingInfo);
						break;
					case NoteData.ScoringType.SliderTail:
						if (!Plugin.Settings.CountArcs) break;
						noteSwingInfo.score = new Score(null, postSwing, offset);
						_noteInfos.Add(noteSwingInfo);
						break;
					case NoteData.ScoringType.BurstSliderHead:
						if (!Plugin.Settings.CountChains) break;
						noteSwingInfo.score = new Score(preSwing, null, offset);
						_noteInfos.Add(noteSwingInfo);
						break;
				}

				_noteSwingInfos.Remove(goodScoringElement.noteData);
			}
			else {
				// Bad cut, do nothing
			}
		}
	}
}

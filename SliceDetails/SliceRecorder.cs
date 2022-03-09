using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using SliceDetails.Data;
using SiraUtil.Logging;

namespace SliceDetails
{
	internal class SliceRecorder : IInitializable, IDisposable
	{
		private readonly BeatmapObjectManager _beatmapObjectManager;
		private readonly SliceProcessor _sliceProcessor;
		private readonly ScoreController _scoreController;

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

			_noteSwingInfos.Add(noteController.noteData, noteInfo);
		}

		public void ScoringForNoteFinishedHandler(ScoringElement scoringElement) {
			NoteInfo noteSwingInfo;
			if (_noteSwingInfos.TryGetValue(scoringElement.noteData, out noteSwingInfo))
			{
				GoodCutScoringElement goodScoringElement = (GoodCutScoringElement)scoringElement;
				if (goodScoringElement.noteData.scoringType == NoteData.ScoringType.Normal)
				{
					IReadonlyCutScoreBuffer cutScoreBuffer = goodScoringElement.cutScoreBuffer;

					int preSwing = cutScoreBuffer.beforeCutScore;
					int postSwing = cutScoreBuffer.afterCutScore;
					int offset = cutScoreBuffer.centerDistanceCutScore;

					noteSwingInfo.score = new Score(preSwing, postSwing, offset);

					_noteInfos.Add(noteSwingInfo);
					_noteSwingInfos.Remove(goodScoringElement.noteData);
				}
			}
			else {
				// Bad cut, do nothing
			}
		}
	}
}

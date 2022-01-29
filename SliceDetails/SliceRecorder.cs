using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using SliceDetails.Data;
using SiraUtil.Logging;

namespace SliceDetails
{
	internal class SliceRecorder : IInitializable, IDisposable, ISaberSwingRatingCounterDidFinishReceiver
	{
		private readonly BeatmapObjectManager _beatmapObjectManager;
		private readonly SliceProcessor _sliceProcessor;
		private readonly SiraLog _siraLog;

		private Dictionary<ISaberSwingRatingCounter, NoteInfo> _noteSwingInfos = new Dictionary<ISaberSwingRatingCounter, NoteInfo>();
		private List<NoteInfo> _noteInfos = new List<NoteInfo>();

		public SliceRecorder(SiraLog siraLog, BeatmapObjectManager beatmapObjectManager, SliceProcessor sliceProcessor) {
			_siraLog = siraLog;
			_beatmapObjectManager = beatmapObjectManager;
			_sliceProcessor = sliceProcessor;
		}

		public void Initialize() {
			_beatmapObjectManager.noteWasCutEvent += OnNoteWasCut;
			_sliceProcessor.ResetProcessor();
		}

		public void Dispose() {
			_beatmapObjectManager.noteWasCutEvent -= OnNoteWasCut;
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
			_siraLog.Info(cutAngle);

			float cutOffset = noteCutInfo.cutDistanceToCenter;
			Vector3 noteCenter = noteController.noteTransform.position;
			if (Vector3.Dot(noteCutInfo.cutNormal, noteCutInfo.cutPoint - noteCenter) > 0f)
			{
				cutOffset = -cutOffset;
			}

			NoteInfo noteInfo = new NoteInfo(noteController.noteData, noteCutInfo, cutAngle, cutOffset, noteGridPosition, noteIndex);

			_noteSwingInfos.Add(noteCutInfo.swingRatingCounter, noteInfo);

			noteCutInfo.swingRatingCounter.UnregisterDidFinishReceiver(this);
			noteCutInfo.swingRatingCounter.RegisterDidFinishReceiver(this);
		}

		public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter) {
			NoteInfo noteSwingInfo;
			if (_noteSwingInfos.TryGetValue(saberSwingRatingCounter, out noteSwingInfo))
			{
				int preSwing, postSwing, offset;
				ScoreModel.RawScoreWithoutMultiplier(saberSwingRatingCounter, noteSwingInfo.cutInfo.cutDistanceToCenter, out preSwing, out postSwing, out offset);

				noteSwingInfo.score = new Score(preSwing, postSwing, offset);

				_noteInfos.Add(noteSwingInfo);
				_noteSwingInfos.Remove(saberSwingRatingCounter);
			}
			else {
				// Bad cut, do nothing
			}
		}
	}
}

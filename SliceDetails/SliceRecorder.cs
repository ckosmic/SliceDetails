using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BeatmapSaveData;
using Zenject;

namespace SliceDetails
{
	internal class SliceRecorder : IInitializable, IDisposable, ISaberSwingRatingCounterDidFinishReceiver
	{
		public static SliceRecorder instance { get; private set; }

		private BeatmapObjectManager _beatmapObjectManager;

		private Dictionary<ISaberSwingRatingCounter, NoteInfo> _noteSwingInfos = new Dictionary<ISaberSwingRatingCounter, NoteInfo>();
		private List<NoteInfo> _noteInfos = new List<NoteInfo>();

		public SliceRecorder(BeatmapObjectManager beatmapObjectManager) {
			_beatmapObjectManager = beatmapObjectManager;
		}

		public void Initialize() {
			instance = this;
			_beatmapObjectManager.noteWasCutEvent += OnNoteWasCut;
			SliceProcessor.instance.ResetProcessor();
		}

		public void Dispose() {
			_beatmapObjectManager.noteWasCutEvent -= OnNoteWasCut;
			// Process slices once the map ends
			ProcessSlices();
		}

		public void ProcessSlices() {
			SliceProcessor.instance.ProcessSlices(_noteInfos);
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

			Vector3 cutDirection = new Vector3(-noteCutInfo.cutNormal.y, noteCutInfo.cutNormal.x, 0f);
			float cutAngle = Mathf.Atan2(cutDirection.y, cutDirection.x) * Mathf.Rad2Deg + 90.0f;

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

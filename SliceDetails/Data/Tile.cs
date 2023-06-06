using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SliceDetails.Data
{
	internal class Tile
	{
		public List<NoteInfo>[] tileNoteInfos = new List<NoteInfo>[18];
		public float[] angleAverages = new float[18];
		public float[] offsetAverages = new float[18];
        public float[] offsetDeviation = new float[18];
        public Score[] scoreAverages = new Score[18];
		public int[] noteCounts = new int[18];
		public float scoreAverage = 0.0f;
		public bool atLeastOneNote = false;
		public int noteCount = 0;

		public void CalculateAverages() {
			angleAverages = new float[18];
			offsetAverages = new float[18];
			offsetDeviation = new float[18];
			scoreAverages = new Score[18];
			noteCounts = new int[18];
			scoreAverage = 0.0f;
			atLeastOneNote = false;

			for (int i = 0; i < 18; i++) {
				scoreAverages[i] = new Score(0.0f, 0.0f, 0.0f);
			}

			noteCount = 0;
			for (int i = 0; i < tileNoteInfos.Length; i++) {
				if (tileNoteInfos[i].Count > 0) {
					int preSwingCount = 0;
					int postSwingCount = 0;
					Vector2 angleXYAverages = Vector2.zero;
					foreach (NoteInfo noteInfo in tileNoteInfos[i]) {
						atLeastOneNote = true;
						angleXYAverages.x += Mathf.Cos(noteInfo.cutAngle * Mathf.PI / 180f);
						angleXYAverages.y += Mathf.Sin(noteInfo.cutAngle * Mathf.PI / 180f);
						offsetAverages[i] += noteInfo.cutOffset;
						offsetDeviation[i] += noteInfo.cutOffset * noteInfo.cutOffset;
						scoreAverages[i] += noteInfo.score;
						noteCounts[i]++;
						scoreAverage += noteInfo.score.TotalScore;
						preSwingCount += scoreAverages[i].CountPreSwing ? 1 : 0;
						postSwingCount += scoreAverages[i].CountPostSwing ? 1 : 0;
						noteCount++;
					}
					angleXYAverages.x /= tileNoteInfos[i].Count;
					angleXYAverages.y /= tileNoteInfos[i].Count;
					angleAverages[i] = Mathf.Atan2(angleXYAverages.y, angleXYAverages.x) * 180f / Mathf.PI;
					offsetDeviation[i] = (offsetDeviation[i] - offsetAverages[i] * offsetAverages[i] / tileNoteInfos[i].Count) / tileNoteInfos[i].Count;
					offsetDeviation[i] = Mathf.Sqrt(offsetDeviation[i]);
					offsetAverages[i] /= tileNoteInfos[i].Count;
					scoreAverages[i].PreSwing /= preSwingCount;
					scoreAverages[i].PostSwing /= postSwingCount;
					scoreAverages[i].Offset /= tileNoteInfos[i].Count;
				}
			}
			scoreAverage /= noteCount;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SliceDetails
{
	internal class Tile
	{
		public List<NoteInfo>[] tileNoteInfos = new List<NoteInfo>[18];
		public float[] angleAverages = new float[18];
		public float[] offsetAverages = new float[18];
		public Score[] scoreAverages = new Score[18];
		public float scoreAverage = 0.0f;
		public bool atLeastOneNote = false;

		public void CalculateAverages() {
			angleAverages = new float[18];
			offsetAverages = new float[18];
			scoreAverages = new Score[18];
			scoreAverage = 0.0f;
			atLeastOneNote = false;

			for (int i = 0; i < 18; i++) {
				scoreAverages[i] = new Score(0.0f, 0.0f, 0.0f);
			}

			int noteCount = 0;
			for (int i = 0; i < tileNoteInfos.Length; i++) {
				if (tileNoteInfos[i].Count > 0) {
					foreach (NoteInfo noteInfo in tileNoteInfos[i]) {
						atLeastOneNote = true;
						angleAverages[i] += noteInfo.cutAngle;
						offsetAverages[i] += noteInfo.cutOffset;
						scoreAverages[i] += noteInfo.score;
						scoreAverage += noteInfo.score.TotalScore;
						noteCount++;
					}
					angleAverages[i] /= tileNoteInfos[i].Count;
					offsetAverages[i] /= tileNoteInfos[i].Count;
					scoreAverages[i] /= tileNoteInfos[i].Count;
				}
			}
			scoreAverage /= noteCount;
		}
	}
}

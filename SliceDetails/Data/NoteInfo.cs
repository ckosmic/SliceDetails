using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SliceDetails.Data
{
	internal class NoteInfo
	{
		public NoteData noteData;
		public NoteCutInfo cutInfo;
		public float cutAngle;
		public float cutOffset;
		public Score score;
		public Vector2 noteGridPosition;
		public int noteIndex;

		public NoteInfo() { 
			
		}

		public NoteInfo(NoteData noteData, NoteCutInfo cutInfo, float cutAngle, float cutOffset, Vector2 noteGridPosition, int noteIndex) {
			this.noteData = noteData;
			this.cutInfo = cutInfo;
			this.cutAngle = cutAngle;
			this.cutOffset = cutOffset;
			this.noteGridPosition = noteGridPosition;
			this.noteIndex = noteIndex;
		}
	}
}

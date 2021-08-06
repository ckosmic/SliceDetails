using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SliceDetails
{
	internal class Score
	{
		public float PreSwing { get; set; }
		public float PostSwing { get; set; }
		public float Offset { get; set; }
		public float TotalScore { 
			get
			{
				return PreSwing + PostSwing + Offset;
			}
		}

		public Score(float preSwing, float postSwing, float offset) {
			PreSwing = preSwing;
			PostSwing = postSwing;
			Offset = offset;
		}

		public static Score operator +(Score a, Score b) => new Score(a.PreSwing + b.PreSwing, a.PostSwing + b.PostSwing, a.Offset + b.Offset);
		public static Score operator /(Score a, float b) => new Score(a.PreSwing / b, a.PostSwing / b, a.Offset / b);
	}
}

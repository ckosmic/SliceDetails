using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SliceDetails.Utils
{
	internal class ResourceUtilities
	{
		public static Sprite LoadSpriteFromResource(string path) {
			Assembly assembly = Assembly.GetCallingAssembly();
			using (Stream stream = assembly.GetManifestResourceStream(path)) {
				if (stream != null) {
					byte[] data = new byte[stream.Length];
					stream.Read(data, 0, (int)stream.Length);
					Texture2D tex = new Texture2D(2, 2);
					if (tex.LoadImage(data)) {
						Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100);
						return sprite;
					}
				} else {
					Plugin.Log.Error("Failed to open sprite resource stream.");
					return null;
				}
			}
			return null;
		}
	}
}

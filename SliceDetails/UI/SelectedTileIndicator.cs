using HMUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeatSaberMarkupLanguage;

namespace SliceDetails.UI
{
	internal class SelectedTileIndicator : MonoBehaviour
	{
		private static Sprite newRoundRect;
		private List<ImageView> _tileDots = new List<ImageView>();

		public void Initialize(AssetLoader assetLoader) {
			// Create a new sliced sprite using the existing bloq texture so we don't have to make another resource
			if (newRoundRect == null) {
				Vector4 spriteBorder = new Vector4(54, 54, 54, 54);
				Texture2D spriteTexture = assetLoader.spr_roundrect.texture;
				Rect rect = new Rect(0, 0, spriteTexture.width, spriteTexture.height);
				newRoundRect = Sprite.Create(spriteTexture, rect, new Vector2(0.5f, 0.5f), 200, 1, SpriteMeshType.FullRect, spriteBorder);
			}

			ImageView background = new GameObject("Background").AddComponent<ImageView>();
			background.transform.SetParent(transform, false);
			background.rectTransform.localScale = Vector3.one;
			background.rectTransform.localPosition = Vector3.zero;
			background.rectTransform.sizeDelta = new Vector2(15.0f, 12.0f);
			background.sprite = newRoundRect;
			background.type = Image.Type.Sliced;
			background.color = new Color(0.125f, 0.125f, 0.125f, 0.75f);
			background.material = Utilities.ImageResources.NoGlowMat;

			for (int i = 0; i < 12; i++) {
				ImageView tileDot = new GameObject("TileDot").AddComponent<ImageView>();
				tileDot.transform.SetParent(background.transform, false);
				tileDot.rectTransform.localScale = Vector3.one;
				tileDot.rectTransform.localPosition = new Vector3((i % 4) * 3.0f - 4.5f, (i / 4) * 3.0f - 3.0f, 0.0f);
				tileDot.rectTransform.sizeDelta = new Vector2(6.0f, 6.0f);
				tileDot.sprite = assetLoader.spr_dot;
				tileDot.type = Image.Type.Simple;
				tileDot.color = Color.white;
				tileDot.material = Utilities.ImageResources.NoGlowMat;
				_tileDots.Add(tileDot);
			}
		}

		public void SetSelectedTile(int tileIndex) {
			for (int i = 0; i < _tileDots.Count; i++) {
				_tileDots[i].color = (tileIndex == i ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.7f));
			}
		}
	}
}

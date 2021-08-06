using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static PlayerSaveData;

namespace SliceDetails.Utils
{
	internal class ColorSchemeManager
	{
		private static ColorScheme _colorScheme;

		public static ColorScheme GetMainColorScheme() {
			if (_colorScheme == null) {
				ColorSchemesSettings colorSchemesSettings = ReflectionUtil.GetField<PlayerData, PlayerDataModel>(Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault(), "_playerData").colorSchemesSettings;
				_colorScheme = colorSchemesSettings.GetSelectedColorScheme();
			}
			return _colorScheme;
		}
	}
}

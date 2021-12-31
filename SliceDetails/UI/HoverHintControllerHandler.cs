using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SliceDetails.UI
{
	internal class HoverHintControllerHandler
	{
		public HoverHintController hoverHintController {
			get {
				return (_hoverHintControllerCopy == null) ? _hoverHintControllerOriginal : _hoverHintControllerCopy;
			}
		}

		private HoverHintController _hoverHintControllerOriginal;
		private HoverHintController _hoverHintControllerCopy;

		internal void SetOriginalHoverHintController(HoverHintController original) {
			_hoverHintControllerOriginal = original;
		}

		internal void CloneHoverHintController() {
			_hoverHintControllerCopy = UnityEngine.Object.Instantiate(_hoverHintControllerOriginal);
			_hoverHintControllerCopy.transform.SetParent(null);
		}
	}
}

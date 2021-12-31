using HMUI;
using Zenject;

namespace SliceDetails.UI
{
	internal class HoverHintControllerGrabber : IInitializable
	{
		private readonly HoverHintControllerHandler _hoverHintControllerHandler;

		private HoverHintController _hoverHintController;

		public HoverHintControllerGrabber(HoverHintControllerHandler hoverHintControllerHandler, HoverHintController hoverHintController) {
			_hoverHintControllerHandler = hoverHintControllerHandler;
			_hoverHintController = hoverHintController;
		}

		public void Initialize() {
			_hoverHintControllerHandler.SetOriginalHoverHintController(_hoverHintController);
		}
	}
}

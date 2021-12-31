using SliceDetails.UI;
using Zenject;

namespace SliceDetails.Installers
{
	public class SDAppInstaller : Installer<SDAppInstaller>
	{
		public override void InstallBindings() {
			Container.Bind<AssetLoader>().AsSingle().Lazy();
			Container.Bind<HoverHintControllerHandler>().AsSingle();
			Container.Bind<SliceProcessor>().AsSingle();
		}
	}
}

using SliceDetails.AffinityPatches;
using SliceDetails.UI;
using Zenject;

namespace SliceDetails.Installers
{
	internal class SDMenuInstaller : Installer<SDMenuInstaller>
	{
		public override void InstallBindings() {
			Container.BindInterfacesAndSelfTo<HoverHintControllerGrabber>().AsSingle();
			Container.Bind<GridViewController>().FromNewComponentAsViewController().AsSingle();
			Container.Bind<UICreator>().AsSingle();

			Container.BindInterfacesTo<ResultsViewControllerPatch>().AsSingle();
			Container.BindInterfacesTo<SoloFreePlayFlowCoordinatorPatch>().AsSingle();
		}
	}
}

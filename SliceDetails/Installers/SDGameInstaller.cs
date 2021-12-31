using SliceDetails.AffinityPatches;
using SliceDetails.UI;
using Zenject;

namespace SliceDetails.Installers
{
	internal class SDGameInstaller : Installer<SDGameInstaller>
	{
		public override void InstallBindings() {
			Container.BindInterfacesAndSelfTo<SliceRecorder>().AsSingle();
			Container.Bind<GridViewController>().FromNewComponentAsViewController().AsSingle();
			Container.Bind<UICreator>().AsSingle();
			Container.BindInterfacesAndSelfTo<PauseUIController>().AsSingle();

			Container.BindInterfacesAndSelfTo<PauseMenuManagerPatches>().AsSingle();
			Container.BindInterfacesAndSelfTo<MenuTransitionsHelperPatch>().AsSingle();
		}
	}
}

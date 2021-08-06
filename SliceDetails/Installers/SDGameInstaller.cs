using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace SliceDetails.Installers
{
	internal class SDGameInstaller : Installer<SDGameInstaller>
	{
		public override void InstallBindings() {
			Container.BindInterfacesAndSelfTo<SliceRecorder>().AsSingle();
			Container.BindInterfacesAndSelfTo<PauseUIController>().AsSingle();
		}
	}
}

using RankedPlaylistGenerator.Managers;
using Zenject;

namespace RankedPlaylistGenerator.Installers
{
    public class MenuInstaller: Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        }
    }
}
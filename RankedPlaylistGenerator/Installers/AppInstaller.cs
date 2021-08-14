using RankedPlaylistGenerator.Managers;
using Zenject;

namespace RankedPlaylistGenerator.Installers
{
    public class AppInstaller: Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<PlaylistGenerator>().AsSingle();
        }
    }
}
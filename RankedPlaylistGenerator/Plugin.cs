
using IPA;
using IPA.Loader;
using SiraUtil;
using SiraUtil.Zenject;

namespace RankedPlaylistGenerator
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        [Init]
        public Plugin(IPA.Logging.Logger logger, IPA.Config.Config config, Zenjector injector, PluginMetadata metadata)
        {
            injector.On<PCAppInit>().Pseudo(container =>
            {
                container.BindLoggerAsSiraLogger(logger);
            });
            injector.OnApp<Installers.AppInstaller>();
            injector.OnMenu<Installers.MenuInstaller>();
        }
    }
}
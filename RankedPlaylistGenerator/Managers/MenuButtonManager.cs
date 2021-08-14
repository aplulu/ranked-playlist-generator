using System;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using SiraUtil.Tools;
using Zenject;

namespace RankedPlaylistGenerator.Managers
{
    internal class MenuButtonManager: IInitializable, IDisposable
    {
        private readonly SiraLog _log = null;
        private readonly MenuButton _menuButton = null;

        [Inject]
        public MenuButtonManager(SiraLog log, PlaylistGenerator playlistGenerator)
        {
            _log = log;
            var name = "Ranked Playlist";
            _menuButton = new MenuButton(name, "Generate ScoreSaber Ranked Playlists", () =>
            {
                Task.Run(async () =>
                {
                    _menuButton.Text = "Generating...";
                    try
                    {
                        await playlistGenerator.Generate();
                        _menuButton.Text = "Generated!";
                    }
                    catch (Exception e)
                    {
                        _log.Logger.Error("Generate Error: " + e.Message);
                        _menuButton.Text = "ERROR";
                    }
                    finally
                    {
                        await Task.Delay(3000);
                        _menuButton.Text = name;

                    }
                });
            }, true);
        }

        public void Initialize()
        {

            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (BSMLParser.IsSingletonAvailable && MenuButtons.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }
    }
}
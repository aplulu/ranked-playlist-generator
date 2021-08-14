using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Policy;
using System.Threading.Tasks;
using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Blist;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using RankedPlaylistGenerator.Models;
using RankedPlaylistGenerator.Utils;
using SiraUtil.Tools;
using UnityEngine.Networking;

namespace RankedPlaylistGenerator.Managers
{
    public class PlaylistGenerator
    {
        private readonly string _beatStarURL = "https://cdn.wes.cloud/beatstar/bssb/v2-all.json.gz";
        
        private readonly SiraLog _log = null;
        
        public PlaylistGenerator(SiraLog log)
        {
            _log = log;
        }

        public async Task Generate()
        {
            _log.Logger.Info("Generate");
            var entries = await downloadBeatStar();
            
            var playlistManager = PlaylistManager.DefaultManager;

            var songByStar = new Dictionary<int, Dictionary<string, IPlaylistSong>>();
            foreach (var kv in entries)
            {
                foreach (var diff in kv.Value.diffs)
                {
                    if (diff.star == 0)
                    {
                        continue;
                    }

                    var star = (int)Math.Truncate(diff.star);
                    if (!songByStar.ContainsKey(star))
                    {
                        songByStar[star] = new Dictionary<string, IPlaylistSong >();
                    }

                    IPlaylistSong song = null;
                    if (songByStar[star].ContainsKey(kv.Key))
                    {
                        song = songByStar[star][kv.Key];
                    }
                    else
                    {
                        song = new LegacyPlaylistSong();
                        song.Hash = kv.Key;
                        song.Difficulties = new List<Difficulty>();
                        songByStar[star][kv.Key] = song;
                    }

                    var difficulty = new Difficulty
                    {
                        Characteristic =
                            ((BeatStarCharacteristicName) Enum.ToObject(typeof(BeatStarCharacteristicName), diff.type))
                            .ToString(),
                        Name = diff.diff == "Expert+" ? "ExpertPlus" : diff.diff
                    };

                    song.Difficulties?.Add(difficulty);

                }
            }

            foreach (var kv in songByStar)
            {
                var fileName = $"ranked_star_{kv.Key:00}";
                var title = $"Ranked Songs ★{kv.Key}";
                _log.Logger.Info($"Writing Playlist {title} ({kv.Value.Count} songs) to {fileName}");
                //var playlist = new BlistPlaylist(fileName, title, "RankedPlaylistGenerator");
                var playlist = new LegacyPlaylist(fileName, title, "RankedPlaylistGenerator");
                playlist.SuggestedExtension = "json";
                playlist.SetCover(ResourceUtil.GetCoverImage($"RankedPlaylistGenerator.Resources.images.{kv.Key}.png"));
                //var playlist = playlistManager.CreatePlaylist(fileName, title, "RankedPlaylistGenerator", ResourceUtil.GetCoverImageLazy($"RankedPlaylistGenerator.Resources.images.{kv.Key}.png"));
                
                foreach (var song in kv.Value.Values)
                {
                    playlist.Add(song);
                }
                playlistManager.StorePlaylist(playlist);
            }
        }


        private async Task<Dictionary<string, BeatStarEntry>> downloadBeatStar()
        {
            _log.Logger.Info("Requesting " + _beatStarURL);
            var req = new UnityWebRequest(_beatStarURL, "GET");
            req.timeout = 10;
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("User-Agent", "RankedPlaylistGenerator/1.0 (+https://github.com/aplulu/ranked-playlist-generator)");
            await req.SendWebRequest();
            if (req.isNetworkError)
            {
                throw new Exception("network error: " + req.error);
            } else if (req.isHttpError)
            {
                throw new Exception("http error: " + req.error);
            }

            using (var reader = new StreamReader(new GZipStream(new MemoryStream(req.downloadHandler.data), CompressionMode.Decompress)))
            {
                var json = reader.ReadToEnd();
                var songs = JsonConvert.DeserializeObject<Dictionary<string, BeatStarEntry>>(json);
                return songs;
            }
        }
    }
}
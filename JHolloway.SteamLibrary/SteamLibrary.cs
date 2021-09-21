using System;
using System.Runtime;
using System.IO;
using System.Collections.Generic;

using Microsoft.Win32;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;

namespace JHolloway.SteamLibrary
{
    public partial class SteamLibrary
    {
        public string? LibraryPath { get; protected set; }
        public string? SteamAppsPath { get; protected set; }
        public SteamGame[] Games { get; protected set; }

        public SteamLibrary(string path = "")
        {
            this.LibraryPath = path;
            if (string.IsNullOrEmpty(path))
            {
                this.Games = Array.Empty<SteamGame>();
                return;
            }

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Steam library not found at path: {path}");

            this.SteamAppsPath = Path.Join(this.LibraryPath, "steamapps");

            List<SteamGame> games = new();
            string[] manifestFiles = Directory.GetFiles(SteamAppsPath, "appmanifest_*.acf");
            foreach (string manifestPath in manifestFiles)
            {
                try
                {
                    VProperty manifest = VdfConvert.Deserialize(File.ReadAllText(manifestPath));
                    SteamGame game = new(manifest.Value["appid"]?.Value<uint>() ?? 0, this, manifest);
                    games.Add(game);
                }
                catch
                {
                    throw;
                }
            }
            this.Games = games.ToArray();
        }
    }
}
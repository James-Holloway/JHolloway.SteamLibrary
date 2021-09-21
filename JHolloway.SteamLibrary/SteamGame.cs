using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gameloop.Vdf.Linq;

namespace JHolloway.SteamLibrary
{
    public class SteamGame
    {
        public uint AppId { get; protected set; }
        public bool Installed => true;
        public string Name;
        public string? InstallPath { get; protected set; }
        public VToken? Manifest { get; protected set; }
        public SteamLibrary? Library;

        public SteamGame(uint appId, SteamLibrary? library, VProperty manifest)
        {
            this.AppId = appId;
            this.Library = library;

            if (manifest.Key.ToLower() == "appstate")
                this.Manifest = manifest.Value;
            else
                this.Manifest = manifest;

            this.Name = this.Manifest?["name"]?.Value<string>() ?? "";

            if (Library != null)
            {
                string installdir = this.Manifest?["installdir"]?.Value<string>() ?? "";
                if (!string.IsNullOrEmpty(installdir))
                {
                    this.InstallPath = Path.Join(Library.SteamAppsPath, "common", installdir);
                }
            }
        }

        public override string ToString()
        {
            return $"SteamGame \"{Name}\"[{AppId}]";
        }
    }
}

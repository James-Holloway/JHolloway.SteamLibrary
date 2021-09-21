using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gameloop.Vdf;
using Gameloop.Vdf.Linq;

using Microsoft.Win32;

namespace JHolloway.SteamLibrary
{
    public partial class SteamLibrary
    {
        public static string GetSteamInstallLocation()
        {
            if (OperatingSystem.IsWindows())
            {
                string regPath = Environment.Is64BitOperatingSystem ? @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam" : @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";
                string? regValue = Registry.GetValue(regPath, "InstallPath", string.Empty) as string;

                if (string.IsNullOrEmpty(regValue))
                    throw new FileNotFoundException("Registry key not found");

                return regValue;
            }
            else if (OperatingSystem.IsLinux())
            {
                return Environment.ExpandEnvironmentVariables("~/.steam/steam");
            }
            else if (OperatingSystem.IsMacOS())
            {
                return Environment.ExpandEnvironmentVariables("~/Library/Application Support/Steam"); // Unsure, unable to test
            }
            else
            {
                throw new NotSupportedException("Operating System not supported");
            }
        }

        public static SteamLibrary[] GetSteamLibraries()
        {
            return GetSteamLibraries(GetSteamInstallLocation());
        }

        public static SteamLibrary[] GetSteamLibraries(string steamInstall)
        {
            steamInstall = Path.GetFullPath(steamInstall);
            if (!Directory.Exists(steamInstall))
                throw new DirectoryNotFoundException("Steam installation directory not found");

            string vdfPath = Path.Join(steamInstall, "config/libraryfolders.vdf");
            if (!File.Exists(vdfPath))
                throw new FileNotFoundException("Config/libraryfolders not found");

            VProperty libraryFolders = VdfConvert.Deserialize(File.ReadAllText(vdfPath));
            List<SteamLibrary> libraries = new() { new SteamLibrary(steamInstall) };

            foreach (VProperty token in libraryFolders.Value)
            {
                if (!UInt32.TryParse(token.Key, out _))
                    continue; // Not a number (probably "contentstatsid")

                string libraryPath = token.Value["path"]?.Value<string>() ?? "";

                if (!string.IsNullOrEmpty(libraryPath))
                    libraries.Add(new SteamLibrary(libraryPath));
            }

            return libraries.ToArray();
        }
    }
}
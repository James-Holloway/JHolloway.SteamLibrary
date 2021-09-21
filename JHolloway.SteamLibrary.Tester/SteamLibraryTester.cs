using NUnit.Framework;

using JHolloway.SteamLibrary;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace JHolloway.SteamLibrary.Tester
{
    public class SteamLibraryTester
    {
        private readonly bool SteamIsInstalled = true;

        private string expectedWindowsPath = Path.GetFullPath(@"C:\Program Files (x86)\Steam");
        private string expectedLinuxPath = Path.GetFullPath("~/.steam/steam");
        private string expectedMacPath = Path.GetFullPath("~/Library/Application Support/Steam");
        private uint expectedLibraryCount => SteamIsInstalled ? 3u : 0u;

        private uint expectedLibraryPathIndex = 2;
        private string expectedLibraryPath = @"F:\Steam";
        private List<uint> expectedAppIds = new() { 107410, 427520 };

        private string expectedGameName = "Factorio";
        private string expectedGamePath = @"F:\Steam\steamapps\common\Factorio";

        private string uninstalledGameName = "Half-Life";

        [SetUp]
        public void Setup() { }

        [Test(TestOf = typeof(SteamLibrary))]
        public void GetSteamInstallLocation()
        {
            string location = SteamLibrary.GetSteamInstallLocation();

            Assert.True(Directory.Exists(location), "Provided directory doesn't exist");

            if (OperatingSystem.IsWindows() && location != expectedWindowsPath)
                Assert.Inconclusive("Location was not the expected path"); // install path can be different
            if (OperatingSystem.IsLinux() && location != expectedLinuxPath)
                Assert.Inconclusive("Location was not the expected path");
            if (OperatingSystem.IsMacOS() && location != expectedMacPath)
                Assert.Inconclusive("Location was not the expected path");
            Assert.Pass(location);
        }

        [Test(TestOf = typeof(SteamLibrary))]
        public void SteamLibraryCount()
        {
            SteamLibrary[] libraries = SteamLibrary.GetSteamLibraries();
            int libraryCount = libraries.Length;

            Assert.That(libraryCount == expectedLibraryCount);
            Assert.Pass(libraryCount.ToString());
        }

        [Test(TestOf = typeof(SteamLibrary))]
        public void SteamLibraryCheck()
        {
            SteamLibrary[] libraries = SteamLibrary.GetSteamLibraries();

            Assert.That(libraries.Length >= expectedLibraryPathIndex - 1, "expectedLibraryPathIndex exceeded library count");

            Assert.That(libraries[expectedLibraryPathIndex].LibraryPath == expectedLibraryPath);
            Assert.Pass(libraries[expectedLibraryPathIndex].LibraryPath);
        }

        [Test(TestOf = typeof(SteamLibrary))]
        public void SteamLibraryAppCheck()
        {
            SteamLibrary[] libraries = SteamLibrary.GetSteamLibraries();
            SteamLibrary library = libraries[expectedLibraryPathIndex];

            List<SteamGame> games = new(library.Games);

            List<uint> appIds = new List<uint>(from game in games select game.AppId);

            Console.WriteLine("Games:");
            games.ForEach(game => Console.WriteLine("  " + game));

            Assert.That(library.LibraryPath == expectedLibraryPath);
            Assert.That(games.Count == expectedAppIds.Count, "App count was not same as expected");
            Assert.Zero(appIds.Except(expectedAppIds).Count(), "Apps IDs in library were not the same as expected");
            Assert.Pass();
        }

        [Test(TestOf = typeof(SteamLibrary))]
        public void SteamLibraryAppNotInstalled()
        {
            SteamLibrary[] libraries = SteamLibrary.GetSteamLibraries();
            foreach (SteamLibrary library in libraries)
            {
                foreach (SteamGame game in library.Games)
                {
                    if (game.Name == uninstalledGameName)
                    {
                        Assert.Fail($"Game install was found for the game {uninstalledGameName}");
                    }
                }
            }
            Assert.Pass("Game that was not expected to installed was not installed");
        }

        [Test(TestOf = typeof(SteamLibrary))]
        public void SteamLibraryGetInstallLocation()
        {
            if (OperatingSystem.IsWindows())
            {
                string steamLocation;
                try
                {
                    steamLocation = SteamLibrary.GetSteamInstallLocation();
                }
                catch (FileNotFoundException)
                {
                    Assert.Pass("");
                }
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Assert.False(string.IsNullOrEmpty(SteamLibrary.GetSteamInstallLocation()));
                Assert.Pass();
            }
            else
            {
                try
                {
                    SteamLibrary.GetSteamInstallLocation();
                }
                catch (NotSupportedException)
                {
                    Assert.Pass("Unsupported threw the correct error");
                }
            }
        }

        [Test(TestOf = typeof(SteamGame))]
        public void SteamGamePathCheck()
        {
            SteamLibrary[] libraries = SteamLibrary.GetSteamLibraries();
            foreach (SteamLibrary library in libraries)
            {
                foreach (SteamGame game in library.Games)
                {
                    if (game.Name == expectedGameName)
                    {
                        Assert.That(game.InstallPath == expectedGamePath, "Game install path was not the same as expected");
                        Assert.Pass(game.InstallPath);
                    }
                }
            }
            Assert.Inconclusive($"Failed to find game of name \"{expectedGameName}\"");
        }

    }
}
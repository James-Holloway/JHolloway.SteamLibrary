# JHolloway.SteamLibrary

## What is this?

This is a project I created to ease discovery of Steam installation directories, 'libraries', and the games install directories. 

Many users on Steam have custom install directories so games or software can be installed on different drives or partitions. However this often means that programs are unable to detect the games without knowing these libraries in advance. 

This library was written so that programs can automatically find a game's install directory without user input. 

## How does it work?

On Windows there is a registry key that points to the Steam install directory. On Linux and MacOS, we check the default directory. From there, we check the config for `libraryfolders.vdf` which provides the path for each library. Each SteamLibrary class contains an array of SteamGames installed under that directory.

Each library then has a list of `appmanifest_<gameid>.acf` files in the `steamapps` directory. This manifest contains information like game name and its directory name. Each SteamGame class provides the AppId, name, install path and the manifest data.

## Other information

This uses [Gameloop.Vdf](https://github.com/shravan2x/Gameloop.Vdf) to parse the appmanifests and `libraryfolders.vdf`.

Further testing is required for MacOS and Linux. If you are able to get a non-default Steam install location easily and programmatically, please submit an issue or pull request. 

Top level namespace may be subject to change. 


<h1 align="center">
  <img align="center" width="120px" src="https://github.com/caioavidal/OpenCoreMMO/blob/develop/ocmsquare.png?raw=true" target="_blank"  />
  <br>
  OPENCOREMMO</h1>
<p align="center">
  <a href="https://ci.appveyor.com/project/caioavidal/opencoremmo">
  <img align="center" src="https://ci.appveyor.com/api/projects/status/973j1ut05o6r8ggg?svg=true" target="_blank"  />
  </a>
  <a href="https://codecov.io/gh/caioavidal/OpenCoreMMO">
  <img align="center" src="https://codecov.io/gh/caioavidal/OpenCoreMMO/branch/develop/graph/badge.svg" />
</a>
<a href="https://www.codefactor.io/repository/github/caioavidal/opencoremmo"><img  align="center"  src="https://www.codefactor.io/repository/github/caioavidal/opencoremmo/badge" alt="CodeFactor" /></a>
<a href="https://discord.gg/Kazq9z2">
  <img align="center" src="https://badgen.net/badge/icon/discord?icon=discord&label" />
</a>
<a href="https://github.com/caioavidal/opencoremmo/stargazers">
  <img align="center" src="https://img.shields.io/github/stars/caioavidal/opencoremmo?label=stargazers&logoColor=yellow&style=social" />
  </a>
  <a href="https://github.com/caioavidal/OpenCoreMMO/blob/develop/LICENSE">
  <img align="center" src="https://badgen.net/github/license/caioavidal/opencoremmo" />
  </a>
</p>

> Modern free and open-source MMORPG server emulator written in C#.
> <br>To connect to the server, you can use either [OTClient](https://github.com/edubart/otclient) or [OpenTibiaUnity](https://github.com/slavidodo/OpenTibia-Unity) for version 8.6.

## Demo

<p align="center">
  <img width="700" align="center" src="https://github.com/caioavidal/OpenCoreMMO/blob/develop/opencoremmo.gif?raw=true" alt="demo"/>
</p>

## Latest Builds

| Enviroment | Status |
|------------|--------|
|![linux](https://badgen.net/badge/icon/Ubuntu%20Linux%2018.04%20x64?icon=terminal&label&color=orange)|[![Build Status](https://caiovidal.visualstudio.com/OpenCoreMMO/_apis/build/status/caioavidal.OpenCoreMMO%20Ubuntu?branchName=develop)](https://caiovidal.visualstudio.com/OpenCoreMMO/_build/latest?definitionId=3&branchName=develop)|
|![win](https://badgen.net/badge/icon/Windows?icon=windows&label&color=blue)|[![Build status](https://ci.appveyor.com/api/projects/status/973j1ut05o6r8ggg?svg=true)](https://ci.appveyor.com/project/caioavidal/opencoremmo)|
|![mac](https://badgen.net/badge/icon/macOS%20Latest?icon=apple&label&color=purple&list=1)|[![Build Status](https://caiovidal.visualstudio.com/OpenCoreMMO/_apis/build/status/caioavidal.OpenCoreMMO%20MACOS?branchName=develop)](https://caiovidal.visualstudio.com/OpenCoreMMO/_build/latest?definitionId=2&branchName=develop)|
|![win](https://badgen.net/badge/icon/Windows,.NET%205?icon=windows&label&list=1)|[![Build Status](https://caiovidal.visualstudio.com/OpenCoreMMO/_apis/build/status/caioavidal.OpenCoreMMO?branchName=develop)](https://caiovidal.visualstudio.com/OpenCoreMMO/_build/latest?definitionId=1&branchName=develop)        |

## Usage

```sh
download and install .NET 6: https://dotnet.microsoft.com/download/dotnet/6.0
git clone https://github.com/caioavidal/OpenCoreMMO.git
cd src
dotnet run -p "NeoServer.Server.Standalone"
```
When connecting to the self-hosted server for development connect using the following:
1. IP Address: 127.0.0.1
2. Port: 7171
3. Account Name: 1
4. Password: 1

## What we have done so far

- [x] Reading OTB and OTBM File structure
- [x] Loading OTBM Tile Area, Towns and Waypoints
- [x] Loading Spawn Areas
- [ ] Loading OTBM House Tiles
- [x] LogIn and LogOut Player on Game
- [x] All player movements
- [x] Multiplayer connection
- [x] Spawn and Respawn Monsters
- [x] PvM Combat
- [x] Depot
- [x] Chats
- [x] Guilds
- [ ] War System
- [x] Public Channels
  - [x] Loot and Death Channels
  - [x] Vip List
- [ ] PvP Combat
- [x] Party
  - [x] Basics
  - [x] Share Loot
  - [x] Shared Experience
- [x] NPC System
- [x] Lua Scripting
- [x] C# Extensions

## Technologies

* C#
* .Net 6
* Database support: InMemory, MySQL, SQL Server and SQLite
* Console Debug Logging
* XUnit Testing

## Links

* Documentation: https://caioavidal.gitbook.io/opencoremmo/
* Discord Invite: https://discord.gg/Kazq9z2
* Trello Board: https://trello.com/b/JnOJ9yn5/opencoremmo
* TibiaKing Topic: https://tibiaking.com/forums/topic/101402-open-source-tibia-server-c-net-5/

## Author

👤 **Caio Vidal**

* Github: [@caioavidal](https://github.com/caioavidal)
* LinkedIn: [https:\/\/www.linkedin.com\/in\/caiovidal](https:\/\/www.linkedin.com\/in\/caiovidal)

## Show your support

Give a ⭐️ if this project helped you!

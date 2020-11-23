<h1 align="center">OpenCoreMMO</h1>
<p align="center">
  
  <a href="https://travis-ci.com/caioavidal/OpenCoreMMO" target="_blank">
  <img align="center" src="https://travis-ci.com/caioavidal/OpenCoreMMO.svg?branch=develop" target="_blank"  />
  </a>
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
</p>

> The OpenCoreMMO is a free and open-source MMORPG server emulator written in C#. It is based on the [forgottenserver](https://github.com/otland/forgottenserver) (OpenTibia)
> <br>To connect to the server, you can use [OTClient](https://github.com/edubart/otclient) or [OpenTibiaUnity](https://github.com/slavidodo/OpenTibia-Unity) for version 8.6.

## Demo

<p align="center">
  <img width="700" align="center" src="https://github.com/caioavidal/OpenCoreMMO/blob/develop/opencoremmo.gif?raw=true" alt="demo"/>
</p>

## Latest Builds

| Enviroment | Status |
|------------|--------|
|![linux](https://badgen.net/badge/icon/Ubuntu%20Linux%2016.04%20x64?icon=travis&label&color=orange)|[![Build Status](https://travis-ci.com/caioavidal/OpenCoreMMO.svg?branch=develop)](https://travis-ci.com/caioavidal/OpenCoreMMO)|
|![win](https://badgen.net/badge/icon/Windows?icon=windows&label&color=blue)|[![Build status](https://ci.appveyor.com/api/projects/status/973j1ut05o6r8ggg?svg=true)](https://ci.appveyor.com/project/caioavidal/opencoremmo)|
|![mac](https://badgen.net/badge/icon/macOS%20Latest?icon=apple&label&color=purple&list=1)|[![Build Status](https://caiovidal.visualstudio.com/OpenCoreMMO/_apis/build/status/caioavidal.OpenCoreMMO%20MACOS?branchName=develop)](https://caiovidal.visualstudio.com/OpenCoreMMO/_build/latest?definitionId=2&branchName=develop)|
|![win](https://badgen.net/badge/icon/Windows,.NET%20Core%203.1.x?icon=windows&label&list=1)|[![Build Status](https://caiovidal.visualstudio.com/OpenCoreMMO/_apis/build/status/caioavidal.OpenCoreMMO?branchName=develop)](https://caiovidal.visualstudio.com/OpenCoreMMO/_build/latest?definitionId=1&branchName=develop)        |

## Usage

```sh
dotnet run -p "NeoServer.Server.Standalone"
docker run --rm -d -p 8080:8080 -p 38888:38888 -v c:/RavenDb/Data:/opt/RavenDB/Server/RavenData --name RavenDb-WithData -e RAVEN_Setup_Mode=None -e RAVEN_License_Eula_Accepted=true -e RAVEN_Security_UnsecuredAccessAllowed=PrivateNetwork ravendb/ravendb
```

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
- [ ] PvP Combat
- [ ] Load NPC
- [ ] Lua Scripting
- [ ] C# Scripting

## Technologies

* C#
* .Net 5
* RavenDB - NoSQL Database (Moving to MongoDB)
* XUnit Testing

## Author

👤 **Caio Vidal**

* Github: [@caioavidal](https://github.com/caioavidal)
* LinkedIn: [https:\/\/www.linkedin.com\/in\/caiovidal](https:\/\/www.linkedin.com\/in\/caiovidal)
* Discord Invite: https://discord.gg/Kazq9z2
## Show your support

Give a ⭐️ if this project helped you!

<h1 align="center">Welcome to OpenCoreMMO 👋</h1>
<p align="center">
  <a href="https://travis-ci.com/caioavidal/OpenCoreMMO" target="_blank">
  <img align="center" src="https://travis-ci.com/caioavidal/OpenCoreMMO.svg?branch=develop" target="_blank"  />
  </a>
  <a href="https://codecov.io/gh/caioavidal/OpenCoreMMO">
  <img align="center" src="https://codecov.io/gh/caioavidal/OpenCoreMMO/branch/develop/graph/badge.svg" />
</a>
</p>

> The OpenCoreMMO is a free and open-source MMORPG server emulator written in C#. It is based on the [forgottenserver](https://github.com/otland/forgottenserver)
> <br>To connect to the server, you can use [OTClient](https://github.com/edubart/otclient) or [OpenTibiaUnity](https://github.com/slavidodo/OpenTibia-Unity).

## ✨ Demo

<p align="center">
  <img width="700" align="center" src="https://github.com/caioavidal/OpenCoreMMO/blob/develop/opencoremmo.gif?raw=true" alt="demo"/>
</p>



## Usage

```sh
dotnet run -p "NeoServer.Server.Standalone"
docker run --rm -d -p 8080:8080 -p 38888:38888 -v c:/RavenDb/Data:/opt/RavenDB/Server/RavenData -v C:/RavenDB/Config:/opt/RavenDB/config ravendb/ravendb
```

## What we have done so far

- [x] Reading OTB and OTBM File structure
- [x] Loading OTBM Tile Area, Towns and Waypoints
- [ ] Loading OTBM Spawn Areas
- [ ] Loading OTBM House Tiles
- [x] LogIn and LogOut Player on Game
- [x] All player movements
- [x] Multiplayer connection
- [ ] Load Monsters
- [ ] Load NPC
- [ ] Lua Scripting

## Technologies

* C#
* .Net Core 3.1
* ReavenDB - NoSQL Database
* XUnit Testing

## Author

👤 **Caio Vidal**

* Github: [@caioavidal](https://github.com/caioavidal)
* LinkedIn: [@https:\/\/www.linkedin.com\/in\/caiovidal](https://linkedin.com/in/https:\/\/www.linkedin.com\/in\/caiovidal)

## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [readme-md-generator](https://github.com/kefranabg/readme-md-generator)_

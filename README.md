<h1 align="center">Welcome to OpenTibiaMMO 👋</h1>
<p align="center">
  <a href="https://travis-ci.com/caioavidal/OpenTibiaMMO" target="_blank">
  <img align="center" src="https://travis-ci.com/caioavidal/OpenTibiaMMO.svg?branch=develop" target="_blank"  />
  </a>
  <a href="https://codecov.io/gh/caioavidal/OpenTibiaMMO">
  <img align="center" src="https://codecov.io/gh/caioavidal/OpenTibiaMMO/branch/develop/graph/badge.svg" />
</a>
</p>

> It's a `Open Source MMO Server` written in C# .Net Core based on `ForgottenServer`
> <br>You can use the `OTClient 8.6`

## ✨ Demo

<p align="center">
  <img width="700" align="center" src="https://github.com/caioavidal/OpenTibiaMMO/blob/develop/opentibiammo.gif?raw=true" alt="demo"/>
</p>



## Usage

```sh
dotnet run -p "NeoServer.Server.Standalone"
docker run --rm -d -p 8080:8080 -p 38888:38888 -v /opt/RavenDb/Data:/opt/RavenDB/Server/RavenData ravendb/ravendb
```

## What we have done so far

- [x] Reading OTB and OTBM File structure
- [x] Loading OTBM Tile Area, Towns and Waypoints
- [ ] Loading OTBM Spawn Areas
- [ ] Loading OTBM House Tiles
- [x] LogIn and LogOut Player on Game
- [x] Player basic movements
- [ ] Player going down or up stairs
- [x] Multiplayer connection
- [ ] Load Monsters
- [ ] Load NPC
- [ ] Lua Scripting

## Technologies

* C#
* .Net Core 3.1
* ReavenDB - NoSQL Database

## Author

👤 **Caio Vidal**

* Github: [@caioavidal](https://github.com/caioavidal)
* LinkedIn: [@https:\/\/www.linkedin.com\/in\/caiovidal](https://linkedin.com/in/https:\/\/www.linkedin.com\/in\/caiovidal)

## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [readme-md-generator](https://github.com/kefranabg/readme-md-generator)_

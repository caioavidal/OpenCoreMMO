// <copyright file="IMapLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Server.Model.World.Structs;
using NeoServer.Server.World.Map;

namespace NeoServer.Server.Map
{
    public interface IMapLoader
    {
        byte PercentageComplete { get; }
		
        bool HasLoaded(int x, int y, byte z);

		ITile GetTile(Location location);
    }
}
// <copyright file="Spawn.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Server.Model.World.Structs;
using System;

namespace COMMO.Server.Data.Models.Structs
{
    public struct Spawn
    {
        public ushort Id { get; set; }

        public Location Location { get; set; }

        public ushort Radius { get; set; }

        public byte Count { get; set; }

        public TimeSpan Regen { get; set; }
    }
}

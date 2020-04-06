// <copyright file="IThing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts
{
    public delegate void OnThingStateChanged(IThing thingThatChanged, IThingStateChangedEventArgs eventArgs);
    public delegate void OnThingRemoved(IThing thing);
    public interface IThing
    {
        event OnThingStateChanged OnThingChanged;
        event OnThingRemoved OnThingRemoved;

        ushort ThingId { get; }

        byte Count { get; }

        Location Location { get; }

        ITile Tile { get; set; }

        string InspectionText { get; }

        string CloseInspectionText { get; }

        bool CanBeMoved { get; }

        void Added();

        void Removed();
        byte GetStackPosition();
    }
}

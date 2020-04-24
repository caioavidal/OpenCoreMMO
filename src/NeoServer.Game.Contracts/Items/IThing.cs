using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums.Location.Structs;
using System;

namespace NeoServer.Game.Contracts.Items
{
    public interface IThing
    {
        Location Location { get; }

         string Name { get; }
        string InspectionText { get; }
        string CloseInspectionText { get; }
    }
}
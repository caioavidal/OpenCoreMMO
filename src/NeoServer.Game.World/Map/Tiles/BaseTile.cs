using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Contracts.Bases;

namespace NeoServer.Game.World.Map.Tiles
{
    public abstract class BaseTile : Store, ITile
    {
        public Location Location { get; protected set; }

        public abstract IItem TopItemOnStack { get; }
        public abstract ICreature TopCreatureOnStack { get; }
        public abstract bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition);

        public abstract byte GetCreatureStackPositionIndex(IPlayer observer);
        
    }
}

using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.World.Map.Tiles
{
    public abstract class BaseTile : ITile
    {
        public Location Location { get; protected set; }

        public abstract IItem TopItemOnStack { get; }
        public abstract ICreature TopCreatureOnStack { get; }
        public abstract bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition);
    }
}

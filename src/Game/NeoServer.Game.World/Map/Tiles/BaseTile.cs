using NeoServer.Game.Common.Contracts.Bases;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location.Structs;

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
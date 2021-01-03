using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Bases;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Server.Model.Players.Contracts;

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

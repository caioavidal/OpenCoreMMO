using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.World
{
    public interface ITile : IStore
    {
        Location Location { get; }

        /// <summary>
        /// check whether tile is 1 sqm distant to destination tile
        /// </summary>
        /// <returns></returns>
        public bool IsNextTo(ITile dest) => Location.IsNextTo(dest.Location);
        bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition);
        byte GetCreatureStackPositionIndex(IPlayer observer);

        IItem TopItemOnStack { get; }
        ICreature TopCreatureOnStack { get; }

    }
}

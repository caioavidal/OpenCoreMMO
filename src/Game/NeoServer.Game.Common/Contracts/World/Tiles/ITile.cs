using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.World.Tiles
{
    public interface ITile : IStore
    {
        Location.Structs.Location Location { get; }

        IItem TopItemOnStack { get; }
        ICreature TopCreatureOnStack { get; }

        /// <summary>
        ///     check whether tile is 1 sqm distant to destination tile
        /// </summary>
        /// <returns></returns>
        public bool IsNextTo(ITile dest)
        {
            return Location.IsNextTo(dest.Location);
        }

        bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition);
        byte GetCreatureStackPositionIndex(IPlayer observer);
    }
}
using System.Collections.Generic;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts
{
    public interface IMap
    {
        ITile this[Location location] { get; }
        ITile this[ushort x, ushort y, sbyte z] { get; }

        void AddPlayerOnMap(ICreature player);
        IList<byte> GetDescription(IThing thing, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        ITile GetNextTile(Location fromLocation, Direction direction);
        IEnumerable<ITile> GetTilesNear(Location location);
    }
}

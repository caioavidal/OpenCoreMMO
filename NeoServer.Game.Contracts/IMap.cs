using System.Collections.Generic;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts
{
    public interface IMap
    {
        ITile this[Location location] { get; }
        ITile this[ushort x, ushort y, sbyte z] { get; }

        void AddPlayerOnMap(IPlayer player);
        IList<byte> GetDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        IList<byte> GetFloorDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte width, byte height, int verticalOffset, ref int skip);
        IList<byte> GetTileDescription(IPlayer player, ITile tile);
        IEnumerable<ITile> GetTilesNear(Location location);
    }
}

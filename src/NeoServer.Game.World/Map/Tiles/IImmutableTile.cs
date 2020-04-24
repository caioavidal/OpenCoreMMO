using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.World.Map.Tiles
{
    public interface IImmutableTile:ITile
    {
        byte[] Raw { get; }

        byte[] GetRaw(IItem[] items);
    }
}
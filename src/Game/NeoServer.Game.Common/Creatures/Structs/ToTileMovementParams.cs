using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Common.Creatures.Structs;

public readonly ref struct ToTileMovementParams
{
    public ToTileMovementParams(IStore source, ITile destination, IItem item, byte amount)
    {
        Source = source;
        Destination = destination;
        Item = item;
        Amount = amount;
    }

    public IStore Source { get; }
    public IStore Destination { get; }
    public IItem Item { get; }
    public byte Amount { get; }
}
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Common.Creatures.Structs;

public readonly ref struct ToTileMovementParams
{
    public ToTileMovementParams(IHasItem source, IDynamicTile destination, IItem item, byte amount)
    {
        Source = source;
        Destination = destination;
        Item = item;
        Amount = amount;
    }

    public IHasItem Source { get; }
    public IHasItem Destination { get; }
    public IItem Item { get; }
    public byte Amount { get; }
}
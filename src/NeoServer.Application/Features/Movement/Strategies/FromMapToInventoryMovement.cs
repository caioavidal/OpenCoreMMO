using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Services;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class FromMapToInventoryMovement : IItemMovement
{
    private readonly IItemMovementService _itemMovementService;
    private readonly IMap _map;

    public FromMapToInventoryMovement(IItemMovementService itemMovementService, IMap map)
    {
        _itemMovementService = itemMovementService;
        _map = map;
    }

    public void Handle(IPlayer player, ItemThrowPacket itemThrow)
    {
        FromMapToInventory(player, _map, itemThrow);
    }

    public string MovementKey => $"{LocationType.Ground.ToString()}-{LocationType.Slot.ToString()}";

    private void FromMapToInventory(IPlayer player, IMap map, ItemThrowPacket itemThrow)
    {
        if (map[itemThrow.FromLocation] is not { } fromTile) return;
        if (fromTile.TopItemOnStack is not { } item) return;
        if (fromTile is not IDynamicTile dynamicTile) return;

        var result = _itemMovementService.Move(player, item, dynamicTile, player.Inventory, itemThrow.Count, 0,
            (byte)itemThrow.ToLocation.Slot);

        if (result.Failed) OperationFailService.Send(player, result.Error);
    }
}
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Server.Commands.Movements;

public class ToMapMovementOperation
{
    public static void Execute(IPlayer player, ItemThrowPacket itemThrow, IToMapMovementService toMapMovementService)
    {
        var movementParams = new MovementParams(itemThrow.FromLocation, itemThrow.ToLocation, itemThrow.Count);
        toMapMovementService.Move(player, movementParams);
    }

    public static bool IsApplicable(ItemThrowPacket itemThrowPacket)
    {
        return itemThrowPacket.ToLocation.Type == LocationType.Ground;
    }
}
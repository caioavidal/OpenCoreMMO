using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class FromAnywhereToMapMovement : IItemMovement
{
    private readonly IToMapMovementService _toMapMovementService;

    public FromAnywhereToMapMovement(IToMapMovementService toMapMovementService)
    {
        _toMapMovementService = toMapMovementService;
    }

    public void Handle(IPlayer player, ItemThrowPacket itemThrow)
    {
        var movementParams = new MovementParams(itemThrow.FromLocation, itemThrow.ToLocation, itemThrow.Count);
        _toMapMovementService.Move(player, movementParams);
    }

    public string MovementKey => LocationType.Ground.ToString();
}
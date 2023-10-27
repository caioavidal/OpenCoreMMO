using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class ItemMovementContext
{
    public ItemMovementContext(IEnumerable<IItemMovement> itemMovements)
    {
        foreach (var itemMovement in itemMovements)
        {
            Strategies.Add(itemMovement.MovementKey, itemMovement);
            break;
        }
    }

    private Dictionary<string, IItemMovement> Strategies { get; } = new();

    public void Handle(IPlayer player, ItemThrowPacket itemThrowPacket)
    {
        var key = itemThrowPacket.ToLocation.Type is LocationType.Ground
            ? LocationType.Ground.ToString()
            : $"{itemThrowPacket.FromLocation}{itemThrowPacket.ToLocation}";

        Strategies.TryGetValue(key, out var itemMovement);
        itemMovement?.Handle(player, itemThrowPacket);
    }
}
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public interface IItemMovement
{
    string MovementKey { get; }
    void Handle(IPlayer player, ItemThrowPacket itemThrowPacket);
}
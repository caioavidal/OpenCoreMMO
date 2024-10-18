using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.PacketHandler.Features.Player.Level.Events;

public abstract class PlayerLevelChangedEventHandler : IEventHandler
{
    private readonly IGameServer game;

    protected PlayerLevelChangedEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player, SkillType skillType, int fromLevel, int toLevel)
    {
        if (Guard.IsNull(player)) return;

        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        SendLevelChangeMessage(skillType, connection, fromLevel, toLevel);

        connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));

        connection.Send();
    }

    protected abstract void SendLevelChangeMessage(SkillType skillType, IConnection connection, int fromLevel,
        int toLevel);
}
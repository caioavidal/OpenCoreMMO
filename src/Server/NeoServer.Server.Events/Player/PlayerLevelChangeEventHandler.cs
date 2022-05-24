using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player;

public abstract class PlayerLevelChangeEventHandler
{
    private readonly IGameServer game;

    public PlayerLevelChangeEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player, SkillType skillType, int fromLevel, int toLevel)
    {
        if (Guard.IsNull(player)) return;

        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        SendLevelChangeMessage(skillType, connection, fromLevel, toLevel);

        SendLevelChangeMessage(skillType, connection, toLevel);

        connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));

        connection.Send();
    }

    protected abstract void SendLevelChangeMessage(SkillType skillType, IConnection connection, int fromLevel,
        int toLevel);

    public abstract void SendLevelChangeMessage(SkillType skillType, IConnection connection,
        int toLevel);
}
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player;

public class PlayerLevelAdvancedEventHandler
{
    private readonly IGameServer game;

    public PlayerLevelAdvancedEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player, SkillType skillType, int fromLevel, int toLevel)
    {
        if (Guard.IsNull(player)) return;

        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        SendLevelAdvancedMessage(skillType, connection, fromLevel, toLevel);

        SendSkillAdvancedMessage(skillType, connection, toLevel);

        connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));

        connection.Send();
    }

    private static void SendLevelAdvancedMessage(SkillType skillType, IConnection connection, int fromLevel,
        int toLevel)
    {
        if (skillType != SkillType.Level) return;

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(
            $"You advanced from level {fromLevel} to level {toLevel}.",
            TextMessageOutgoingType.MESSAGE_EVENT_ADVANCE));
    }

    private static void SendSkillAdvancedMessage(SkillType skillType, IConnection connection,
        int toLevel)
    {
        if (skillType == SkillType.Level) return;

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(
            MessageParser.GetSkillAdvancedMessage(skillType, toLevel),
            TextMessageOutgoingType.MESSAGE_EVENT_ADVANCE));
    }
}
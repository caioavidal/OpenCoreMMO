using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player;

public class PlayerLevelRegressedEventHandler : PlayerLevelChangeEventHandler
{
    public PlayerLevelRegressedEventHandler(IGameServer game) : base(game)
    { }

    protected override void SendLevelChangeMessage(SkillType skillType, IConnection connection, int fromLevel, int toLevel)
    {
        if (skillType != SkillType.Level)
            return;

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(
            $"You regressed from level {fromLevel} to level {toLevel}.",
            TextMessageOutgoingType.MESSAGE_EVENT_LEVEL_CHANGE));
    }

    public override void SendLevelChangeMessage(SkillType skillType, IConnection connection, int toLevel)
    {
        if (skillType == SkillType.Level) return;

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(
            MessageParser.GetSkillAdvancedMessage(skillType, toLevel),
            TextMessageOutgoingType.MESSAGE_EVENT_LEVEL_CHANGE));
    }
}
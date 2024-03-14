using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Application.Features.Player.Level.Events;

public class PlayerLevelAdvancedEventHandler : PlayerLevelChangedEventHandler
{
    public PlayerLevelAdvancedEventHandler(IGameServer game) : base(game)
    {
    }

    protected override void SendLevelChangeMessage(SkillType skillType, IConnection connection, int fromLevel,
        int toLevel)
    {
        connection.OutgoingPackets.Enqueue(new TextMessagePacket(
            MessageParser.GetSkillAdvancedMessage(skillType, fromLevel, toLevel),
            TextMessageOutgoingType.MESSAGE_EVENT_LEVEL_CHANGE));
    }
}
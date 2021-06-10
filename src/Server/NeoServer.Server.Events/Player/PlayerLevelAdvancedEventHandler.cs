using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLevelAdvancedEventHandler
    {
        private readonly IGameServer game;

        public PlayerLevelAdvancedEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, SkillType type, int fromLevel, int toLevel)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                if (SkillType.Level == type)
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket(
                        $"You advanced from level {fromLevel} to level {toLevel}.",
                        TextMessageOutgoingType.MESSAGE_EVENT_ADVANCE));
                else
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket(
                        MessageParser.GetSkillAdvancedMessage(type, toLevel),
                        TextMessageOutgoingType.MESSAGE_EVENT_ADVANCE));

                connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));

                connection.Send();
            }
        }
    }
}
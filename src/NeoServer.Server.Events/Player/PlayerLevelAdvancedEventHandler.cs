using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerLevelAdvancedEventHandler
    {
        private readonly Game game;
        public PlayerLevelAdvancedEventHandler(Game game) => this.game = game;
        public void Execute(IPlayer player, SkillType type, int fromLevel, int toLevel)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                if (SkillType.Level == type)
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"You advanced from level {fromLevel} to level {toLevel}.", TextMessageOutgoingType.MESSAGE_EVENT_ADVANCE));
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket(MessageParser.GetSkillAdvancedMessage(type, toLevel), TextMessageOutgoingType.MESSAGE_EVENT_ADVANCE));
                }

                connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));

                connection.Send();
            }

        }
    }
}

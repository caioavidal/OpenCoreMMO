using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Enums;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class PlayerCannotUseSpellEventHandler
    {
        private readonly Game game;

        public PlayerCannotUseSpellEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, ISpell spell, InvalidOperation error)
        {
            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, EffectT.Puff));
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(TextMessageOutgoingParser.Parse(error), TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                connection.Send();
            }
        }
    }
}

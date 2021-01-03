using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class SpellInvokedEventHandler
    {
        private readonly Game game;

        public SpellInvokedEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, ISpell spell)
        {
            foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, spell.Effect));
                connection.Send();
            }
        }
    }
}

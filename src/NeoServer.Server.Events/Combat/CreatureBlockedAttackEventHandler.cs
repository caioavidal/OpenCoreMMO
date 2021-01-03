using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class CreatureBlockedAttackEventHandler
    {
        private readonly Game game;

        public CreatureBlockedAttackEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, BlockType blockType)
        {

            foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                var effect = blockType == BlockType.Armor ? EffectT.SparkYellow : EffectT.Puff;

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, effect));
                connection.Send();
            }
        }
    }
}

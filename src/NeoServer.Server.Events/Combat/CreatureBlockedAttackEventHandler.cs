using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Combat;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;


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

            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                var effect = blockType == BlockType.Armor ? EffectT.SparkYellow : EffectT.Puff;

                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, effect));
                connection.Send();
            }
        }
    }
}

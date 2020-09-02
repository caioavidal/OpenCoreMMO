using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using System.Collections.Generic;

namespace NeoServer.Server.Events
{
    public class CreatureInjuredEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureInjuredEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature enemy, ICreature victim, ushort healthDamage)
        {
            var outgoingPackets = new Queue<IOutgoingPacket>();

            foreach (var spectatorId in map.GetPlayersAtPositionZone(victim.Location))
            {
                var healthDamageString = healthDamage.ToString();

                if (enemy.CreatureId == spectatorId)
                {
                    outgoingPackets.Enqueue(new TextMessagePacket($"{victim.Name} loses {healthDamageString} due to your attack", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                }

                outgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, Enums.Creatures.Enums.EffectT.XBlood));
                outgoingPackets.Enqueue(new AnimatedTextPacket(victim.Location, TextColor.Red, healthDamageString));
                outgoingPackets.Enqueue(new CreatureHealthPacket(victim));

                IConnection connection = null;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                connection.Send(outgoingPackets);

            }

        }
    }
}

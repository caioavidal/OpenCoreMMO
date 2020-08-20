using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
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
            foreach (var spectatorId in map.GetPlayersAtPositionZone(victim.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                if(victim.CreatureId == spectatorId) //myself
                {
                    connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)victim));
                }

                var healthDamageString = healthDamage.ToString();

                if (enemy.CreatureId == spectatorId)
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"{victim.Name} loses {healthDamageString} due to your attack", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, Enums.Creatures.Enums.EffectT.XBlood));
                connection.OutgoingPackets.Enqueue(new AnimatedTextPacket(victim.Location, TextColor.Red, healthDamageString));
                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(victim));

                connection.Send();
            }


        }
    }
}

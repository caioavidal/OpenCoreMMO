using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Parsers.Effects;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Helpers;
using System.Linq;

namespace NeoServer.Server.Events.Combat
{
    public class CreatureAttackEventHandler
    {
        private readonly Game game;

        public CreatureAttackEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, ICreature victim, CombatAttackType attack)
        {
            foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
            {

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }
            
                if (attack.Missed)
                {
                    SendMissedAttack(creature, victim, attack, connection);
                }
                else
                {
                    if (attack.ShootType != default) connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, victim.Location, (byte)attack.ShootType));
                }
                if (attack.Area?.Any() ?? false)
                {
                    foreach (var coordinate in attack.Area)
                    {
                        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(coordinate.Location, DamageEffectParser.Parse(attack.DamageType)));
                    }
                }

                connection.Send();
            }
        }

        private static void SendMissedAttack(ICreature creature, ICreature victim, CombatAttackType attack, IConnection connection)
        {
            var index = 0;
            Location destLocation;
            do
            {
                index = ServerRandom.Random.Next(minValue: 0, maxValue: victim.Location.Neighbours.Length);
                destLocation = victim.Location.Neighbours[index];
            } while (destLocation == creature.Location);

            if (attack.ShootType != default) connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, destLocation, (byte)attack.ShootType));
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(destLocation, EffectT.Puff));
        }
    
    }
}
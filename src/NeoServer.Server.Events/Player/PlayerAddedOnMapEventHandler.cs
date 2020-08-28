using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;

namespace NeoServer.Server.Events
{
    public class PlayerAddedOnMapEventHandler : IEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public PlayerAddedOnMapEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature creature)

        {

            var outgoingPackets = new Queue<IOutgoingPacket>();

            foreach (var spectatorId in map.GetPlayersAtPositionZone(creature.Location))
            {

                var isSpectator = !(creature.CreatureId == spectatorId);
                if (!isSpectator)
                {
                    SendPacketsToPlayer(creature as IPlayer, outgoingPackets);
                }
                else
                {
                    if (!game.CreatureManager.TryGetPlayer(spectatorId, out IPlayer spectator))
                    {
                        continue;
                    }

                    SendPacketsToSpectator(spectator, creature, outgoingPackets);
                }

                IConnection connection;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                connection.Send(outgoingPackets);

            }


        }

        private void SendPacketsToSpectator(IPlayer playerToSend, ICreature creatureAdded, Queue<IOutgoingPacket> outgoingPackets)
        {
            //spectator.add
            outgoingPackets.Enqueue(new AddAtStackPositionPacket(creatureAdded));
            outgoingPackets.Enqueue(new AddCreaturePacket(playerToSend, creatureAdded));
            outgoingPackets.Enqueue(new MagicEffectPacket(creatureAdded.Location, EffectT.BubbleBlue));

        }

        private void SendPacketsToPlayer(IPlayer player, Queue<IOutgoingPacket> outgoingPackets)
        {
            outgoingPackets.Enqueue(new SelfAppearPacket(player));
            outgoingPackets.Enqueue(new MapDescriptionPacket(player, map));
            outgoingPackets.Enqueue(new MagicEffectPacket(player.Location, EffectT.BubbleBlue));
            outgoingPackets.Enqueue(new PlayerInventoryPacket(player.Inventory));
            outgoingPackets.Enqueue(new PlayerStatusPacket(player));
            outgoingPackets.Enqueue(new PlayerSkillsPacket(player));

            outgoingPackets.Enqueue(new WorldLightPacket(game.LightLevel, game.LightColor));

            outgoingPackets.Enqueue(new CreatureLightPacket(player));

            outgoingPackets.Enqueue(new PlayerConditionsPacket());


        }
    }
}

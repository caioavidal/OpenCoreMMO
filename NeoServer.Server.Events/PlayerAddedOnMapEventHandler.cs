using System;
using System.Collections.Generic;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Events;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Schedulers;
using NeoServer.Server.Schedulers.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerAddedOnMapEventHandler : IEventHandler<PlayerAddedOnMapEvent>
    {
        private readonly IMap map;
        private readonly Game game;

        public PlayerAddedOnMapEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(PlayerAddedOnMapEvent evt)
        {
            var player = evt.Player;

            var outgoingPackets = new Queue<IOutgoingPacket>();

            foreach (var spectatorId in map.GetCreaturesAtPositionZone(player.Location))
            {
                var isSpectator = !(player.CreatureId == spectatorId);
                if (!isSpectator)
                {
                    SendPacketsToPlayer(player, outgoingPackets);
                    Console.WriteLine($"player: {player.Name}");
                }
                else
                {
                    var spectator = game.CreatureInstances[spectatorId] as IPlayer;
                    SendPacketsToSpectator(spectator, player, outgoingPackets);
                    Console.WriteLine($"spectator: {spectator.Name}");
                }

                if (game.Connections.TryGetValue(spectatorId, out IConnection connection))
                {
                    connection.Send(outgoingPackets, isSpectator);
                    Console.WriteLine($"Packet sent to {spectatorId}");
                }
            }


        }

        private void SendPacketsToSpectator(IPlayer playerToSend, ICreature creatureAdded, Queue<IOutgoingPacket> outgoingPackets)
        {
            //spectator.add
            outgoingPackets.Enqueue(new AddAtStackPositionPacket((IPlayer)creatureAdded));
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

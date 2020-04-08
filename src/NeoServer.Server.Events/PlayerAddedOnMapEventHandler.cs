using System.Collections.Generic;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Events;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

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
        public void Execute(IPlayer player)

        {

            var outgoingPackets = new Queue<IOutgoingPacket>();

            foreach (var spectatorId in map.GetCreaturesAtPositionZone(player.Location))
            {
                var isSpectator = !(player.CreatureId == spectatorId);
                if (!isSpectator)
                {
                    SendPacketsToPlayer(player, outgoingPackets);
                }
                else
                {
                    var spectator = game.CreatureManager.GetCreature(spectatorId) as IPlayer;
                    SendPacketsToSpectator(spectator, player, outgoingPackets);
                }

                var connection = game.CreatureManager.GetPlayerConnection(spectatorId);

                connection.Send(outgoingPackets, isSpectator);

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

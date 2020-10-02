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


            foreach (var spectatorId in map.GetCreaturesAtPositionZone(creature.Location, creature.Location))
            {

                if (!game.CreatureManager.TryGetCreature(spectatorId, out var spectator))
                {
                    return;
                }

                IConnection connection;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                if (spectator is IMonster monster && creature is IPlayer target)
                {
                    monster.AddToTargetList(target);
                    continue;
                }

                var isSpectator = !(creature.CreatureId == spectatorId);
                if (!isSpectator)
                {
                    SendPacketsToPlayer(creature as IPlayer, connection);
                }
                else
                {
                    if (!game.CreatureManager.TryGetPlayer(spectatorId, out IPlayer spectatorPlayer))
                    {
                        continue;
                    }

                    SendPacketsToSpectator(spectatorPlayer, creature, connection);
                }           
                connection.Send();

            }

        }

        private void SendPacketsToSpectator(IPlayer playerToSend, ICreature creatureAdded, IConnection connection)
        {
            
            //spectator.add
            connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creatureAdded));
            connection.OutgoingPackets.Enqueue(new AddCreaturePacket(playerToSend, creatureAdded));
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creatureAdded.Location, EffectT.BubbleBlue));

        }

        private void SendPacketsToPlayer(IPlayer player, IConnection connection)
        {
            connection.OutgoingPackets.Enqueue(new SelfAppearPacket(player));
            connection.OutgoingPackets.Enqueue(new MapDescriptionPacket(player, map));
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(player.Location, EffectT.BubbleBlue));
            connection.OutgoingPackets.Enqueue(new PlayerInventoryPacket(player.Inventory));
            connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));
            connection.OutgoingPackets.Enqueue(new PlayerSkillsPacket(player));

            connection.OutgoingPackets.Enqueue(new WorldLightPacket(game.LightLevel, game.LightColor));

            connection.OutgoingPackets.Enqueue(new CreatureLightPacket(player));

            connection.OutgoingPackets.Enqueue(new PlayerConditionsPacket());

        }
    }
}

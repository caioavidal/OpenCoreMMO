using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerSelfAppearOnMapEventHandler : IEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public PlayerSelfAppearOnMapEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IWalkableCreature creature)
        {
         

            creature.ThrowIfNull();

            if (!game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection)) return;

            SendPacketsToPlayer(creature as IPlayer, connection);

            connection.Send();

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

            ushort icons = 0;
            foreach (var condition in player.Conditions)
            {
                icons |= (ushort)ConditionIconParser.Parse(condition.Key);
            }

            connection.OutgoingPackets.Enqueue(new ConditionIconPacket(icons));
        }
    }
}

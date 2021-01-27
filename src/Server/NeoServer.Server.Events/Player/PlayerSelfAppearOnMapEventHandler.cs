using NeoServer.Data.Interfaces;
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
        private readonly IAccountRepository accountRepository;

        public PlayerSelfAppearOnMapEventHandler(IMap map, Game game, IAccountRepository accountRepository)
        {
            this.map = map;
            this.game = game;
            this.accountRepository = accountRepository;
        }
        public void Execute(IWalkableCreature creature)
        {
            creature.ThrowIfNull();

            if (creature is not IPlayer player) return;

            if (!game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection)) return;

            SendPacketsToPlayer(player, connection);


            foreach (var loggedPlayer in game.CreatureManager.GetAllLoggedPlayers())
            {
                if (!loggedPlayer.HasInVipList(player.Id)) continue;
                if (!game.CreatureManager.GetPlayerConnection(loggedPlayer.CreatureId, out var friendConnection)) continue;

                friendConnection.OutgoingPackets.Enqueue(new PlayerUpdateVipStatusPacket(player.Id, true));
                friendConnection.Send();
            }
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

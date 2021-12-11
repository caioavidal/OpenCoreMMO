using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Items.Bases;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Networking.Packets.Outgoing.Item;
using NeoServer.Networking.Packets.Outgoing.Window;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player
{
    public class PlayerReadTextEventHandler : IEventHandler
    {
        private readonly IGameServer game;

        public PlayerReadTextEventHandler( IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, IReadable readable, string text)
        {
            if (Guard.AnyNull(player, readable)) return;

            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;
            
            var id = ItemTextWindowStore.Add(player, readable);

            connection.OutgoingPackets.Enqueue(new TextWindowPacket(id, readable));

            connection.Send();
        }
    }
}
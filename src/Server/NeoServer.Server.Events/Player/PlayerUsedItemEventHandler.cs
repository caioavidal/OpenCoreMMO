using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Events
{
    public class PlayerUsedItemEventHandler
    {
        private readonly IGameServer game;

        public PlayerUsedItemEventHandler(IGameServer game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player, IThing onThing, IUseableOn item)
        {
            if (item.Effect == EffectT.None) return;

            foreach (var spectator in game.Map.GetPlayersAtPositionZone(onThing.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(onThing.Location, item.Effect));
                connection.Send();
            }

        }
    }
}

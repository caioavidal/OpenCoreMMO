using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerUsedItemEventHandler
    {
        private readonly Game game;

        public PlayerUsedItemEventHandler(Game game)
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

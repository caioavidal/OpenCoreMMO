using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Creatures;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerUsedItemEventHandler
{
    private readonly IGameServer game;

    public PlayerUsedItemEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player, IThing onThing, IUsableOn item)
    {
        if (item.Effect == EffectT.None) return;

        foreach (var spectator in game.Map.GetPlayersAtPositionZone(onThing.Location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(onThing.Location, item.Effect));
            connection.Send();
        }
    }
}
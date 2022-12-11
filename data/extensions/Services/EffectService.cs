using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Helpers;

namespace NeoServer.Extensions.Services;

public static class EffectService
{
    public static void Send(Location location, EffectT effect)
    {
        var map = IoC.GetInstance<IMap>();
        var game = IoC.GetInstance<IGameServer>();

        foreach (var spectator in map.GetPlayersAtPositionZone(location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(location, effect));
            connection.Send();
        }
    }
}
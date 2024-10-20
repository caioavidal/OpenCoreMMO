using NeoServer.BuildingBlocks.Application;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing.Effect;

namespace NeoServer.Modules.Combat.Attacks.InAreaAttack;

public readonly record struct AreaEffectParam(AffectedLocation2[] Area, EffectT Effect);

public class AreaEffectPacketDispatcher(IMap map, IGameCreatureManager creatureManager): ISingleton
{
    public void Send(ICreature creature, AreaEffectParam param)
    {
        var spectators = map.GetPlayersAtPositionZone(creature.Location);

        foreach (var spectator in spectators)
        {
            if (spectator is not IPlayer) continue;

            if (!creatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;
            foreach (var coordinate in param.Area)
            {
                if (coordinate.Missed) continue;
                SendEffect(param.Effect, coordinate.Point.Location, connection);
            }
            
            connection.Send();
        }
    }

    private static void SendEffect(EffectT effect, Location location, IConnection connection)
    {
        if (effect is EffectT.None) return;

        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(location, effect));
    }
}
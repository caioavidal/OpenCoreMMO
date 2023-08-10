using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Systems.Events;
using NeoServer.Game.World.Algorithms;

namespace NeoServer.Game.Systems.Combat.Attacks;

public static class AreaCombatAttack
{
    private static IMap Map { get; set; }

    public static void Setup(IMap map)
    {
        Map = map;
    }

    public static bool PropagateAttack(ICombatActor actor, CombatAttackParams combatAttackParams)
    {
        var victims = new List<ICombatActor>();

        foreach (var coordinate in combatAttackParams.Area)
        {
            var location = coordinate.Point.Location;
            var tile = Map[location];

            if (tile is not IDynamicTile walkableTile || walkableTile.HasFlag(TileFlags.Unpassable) ||
                walkableTile.ProtectionZone)
            {
                coordinate.MarkAsMissed();
                continue;
            }

            if (!SightClear.IsSightClear(Map, actor.Location, location, false))
            {
                coordinate.MarkAsMissed();
                continue;
            }

            var targetCreatures = walkableTile.Creatures?.ToArray();

            if (targetCreatures is null) continue;

            foreach (var target in targetCreatures)
            {
                if (actor == target) continue;

                if (target is not ICombatActor targetCreature)
                {
                    coordinate.MarkAsMissed();
                    continue;
                }

                victims.Add(targetCreature);
            }
        }

        CombatEvent.InvokeOnAttackingEvent(actor, new[] { combatAttackParams });

        var attackResult = true;

        foreach (var victim in victims)
        {
            foreach (var damage in combatAttackParams.Damages)
            {
                attackResult &= victim.ReceiveAttack(actor, damage);
            }
        }

        return attackResult;
    }
}
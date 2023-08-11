using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.World.Algorithms;

namespace NeoServer.Game.Systems.Combat.Attacks;

public static class AreaCombatAttackProcessor
{
    private static IMap Map { get; set; }
    public static void Setup(IMap map) => Map = map;

    public static Dictionary<uint, CombatAttackParams> LastCombatAttackParamProcessed { get; } = new();

    public static void Process(ICombatActor actor, CombatAttackParams combatAttackParams)
    {
        var victims = new List<ICombatActor>();

        var hasAnyLocationAffected = false;
        foreach (var coordinate in combatAttackParams.Area.AffectedLocations)
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

            if (targetCreatures is null || !targetCreatures.Any())
            {
                hasAnyLocationAffected = true;
                continue;
            }

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

        combatAttackParams.Area.MarkAsProcessed(hasAnyLocationAffected, victims.ToArray());
        
        LastCombatAttackParamProcessed.Remove(actor.CreatureId);
        LastCombatAttackParamProcessed.Add(actor.CreatureId, combatAttackParams);
    }
}
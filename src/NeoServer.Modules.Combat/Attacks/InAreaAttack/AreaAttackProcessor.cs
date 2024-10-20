using NeoServer.BuildingBlocks.Application;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.World.Algorithms;
using NeoServer.Modules.Combat.Attacks.InAreaAttack;
using NeoServer.Modules.Combat.MonsterDefense;
using NeoServer.Modules.Combat.PlayerDefense;

public class AreaAttackProcessor(IMap map, AreaEffectPacketDispatcher areaEffectPacketDispatcher) : ISingleton
{
    public void Propagate(AttackInput attackInput, CombatDamageList combatDamageList)
    {
        var area = attackInput.Parameters.Area;
        var affectedArea = new AffectedLocation2[area.Coordinates.Length];
        var targetCreaturesList = new List<ICombatActor>(); // Store targets and their coordinates

        var index = 0;

        // Phase 1: Collect the affected areas and targets for packet dispatch
        foreach (var coordinate in area.Coordinates)
        {
            var location = coordinate.Location;
            var tile = map[location];

            // Check if the tile is walkable and clear of obstacles
            if (tile is not IDynamicTile walkableTile || walkableTile.HasFlag(TileFlags.Unpassable) || 
                walkableTile.ProtectionZone)
            {
                affectedArea[index++] = new AffectedLocation2(coordinate, missed: true);
                continue;
            }

            // Check if the line of sight is clear between aggressor and target location
            if (!SightClear.IsSightClear(map, attackInput.Aggressor.Location, tile.Location, false))
            {
                affectedArea[index++] = new AffectedLocation2(coordinate, missed: true);
                continue;
            }

            var targetCreatures = walkableTile.Creatures?.ToArray();
            if (targetCreatures is null)
            {
                affectedArea[index++] = new AffectedLocation2(coordinate, missed: false);
                continue;
            }

            // Mark the area as affected and add creatures to the list for defense handling
            affectedArea[index++] = new AffectedLocation2(coordinate, missed: false);

            foreach (var target in targetCreatures)
            {
                if (attackInput.Aggressor == target) continue;

                if (target is ICombatActor targetCreature)
                {
                    targetCreaturesList.Add(targetCreature);
                }
            }
        }

        // Phase 2: Send the packet for the area effect before defense handling
        areaEffectPacketDispatcher.Send(attackInput.Aggressor, new AreaEffectParam
        {
            Area = affectedArea,
            Effect = attackInput.Parameters.Effect
        });

        // Phase 3: Now apply defense logic after sending the packet
        foreach (var targetCreature in targetCreaturesList)
        {
            switch (targetCreature)
            {
                case IPlayer player:
                    PlayerDefenseHandler.Handle(attackInput.Aggressor, player, combatDamageList);
                    break;
                case IMonster monster:
                    MonsterDefenseHandler.Handle(attackInput.Aggressor, monster, combatDamageList);
                    break;
            }
        }
    }
}

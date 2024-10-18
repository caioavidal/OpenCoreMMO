// using NeoServer.Game.Combat;
// using NeoServer.Game.Common.Combat.Structs;
// using NeoServer.Game.Common.Contracts.Creatures;
// using NeoServer.Game.Common.Contracts.World;
// using NeoServer.Game.Common.Contracts.World.Tiles;
// using NeoServer.Game.Common.Location;
// using NeoServer.Game.World.Algorithms;
//
// namespace NeoServer.Application.Features.Combat.Attacks.InAreaAttack;
//
// public class AreaAttack(IMap map)
// {
//     public AffectedLocation2[] Execute(AttackInput attackInput, CombatDamage combatDamage)
//     {
//         return PropagateAttack(attackInput, combatDamage: combatDamage);
//     }
//
//     private AffectedLocation2[] PropagateAttack(AttackInput attackInput, CombatDamage combatDamage)
//     {
//         var affectedArea = new AffectedLocation2[attackInput.Area.Coordinates.Length];
//
//         var index = 0;
//         foreach (var coordinate in attackInput.Area.Coordinates)
//         {
//             var location = coordinate.Location;
//             var tile = map[location];
//         
//             if (tile is not IDynamicTile walkableTile || walkableTile.HasFlag(TileFlags.Unpassable) ||
//                 walkableTile.ProtectionZone)
//             {
//                 affectedArea[index++] = new AffectedLocation2(coordinate, missed: true);
//                 continue;
//             }
//         
//             if (!SightClear.IsSightClear(map, attackInput.Aggressor.Location, attackInput.Target.Location, false))
//             {
//                 affectedArea[index++] = new AffectedLocation2(coordinate, missed: true);
//                 continue;
//             }
//         
//             var targetCreatures = walkableTile.Creatures?.ToArray();
//
//             if (targetCreatures is null)
//             {
//                 affectedArea[index++] = new AffectedLocation2(coordinate, missed: true);
//                 continue;
//             }
//         
//             foreach (var target in targetCreatures)
//             {
//                 if (attackInput.Aggressor == target) continue;
//         
//                 if (target is not ICombatActor targetCreature)
//                 {
//                     affectedArea[index++] = new AffectedLocation2(coordinate, missed: true);
//                     continue;
//                 }
//         
//                 targetCreature.ReceiveAttackFrom(attackInput.Aggressor, combatDamage);
//             }
//             
//             affectedArea[index++] = new AffectedLocation2(coordinate, missed: false);
//         }
//
//         return affectedArea;
//     }
// }


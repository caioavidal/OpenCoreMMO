using NeoServer.Game.Combat.Validation;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Systems.Events;

namespace NeoServer.Game.Systems.Combat.Attacks.Item;

public class AreaRuneCombatAttack : IItemCombatAttack
{
    private static IMapTool MapTool { get; set; }
    private static Func<string, byte[,]> GetAreaTypeFunc { get; set; }

    public static void Setup(IMap map, IMapTool mapTool, Func<string, byte[,]> getAreaTypeFunc)
    {
        MapTool = mapTool;
        GetAreaTypeFunc = getAreaTypeFunc;
    }

    public static IItemCombatAttack Instance { get; } = new AreaRuneCombatAttack();

    public CombatType CombatType => CombatType.Rune;

    public Result CauseDamage(IItem item, IThing aggressor, IThing enemy)
    {
        if (aggressor is not IPlayer player) return Result.NotApplicable;

        if (item is not IAttackRune { NeedTarget: false } rune) return Result.NotApplicable;

        var tileDestination = GetTileDestination(enemy);

        var attackIsValid = AttackIsValid(player, tileDestination);

        if (attackIsValid.Failed) return attackIsValid;

        var combatAttackParams = rune.PrepareAttack(player);

        var template = GetAreaTypeFunc?.Invoke(rune.Area);
        combatAttackParams.SetArea(AreaEffect.Create(tileDestination.Location, template));

        AreaCombatAttack.PropagateAttack(player, combatAttackParams);
        return Result.Success;
    }

    private static ITile GetTileDestination(IThing enemy)
    {
        return enemy switch
        {
            ICreature creature => creature.Tile,
            ITile tile => tile,
            _ => null
        };
    }

    private static Result AttackIsValid(IPlayer player, ITile tile)
    {
        if (tile is not IDynamicTile)
        {
            OperationFailService.Send(player.CreatureId, "There is not enough room.");
            return Result.NotPossible;
        }

        var result = AttackValidation.CanAttackInArea(player, tile);

        if (result.Failed)
        {
            player.StopAttack();
            {
                return result;
            }
        }

        if (MapTool.SightClearChecker?.Invoke(player.Location, tile.Location, true) == false)
        {
            OperationFailService.Send(player.CreatureId, "You cannot throw there.");
            return Result.NotPossible;
        }

        return Result.Success;
    }
}
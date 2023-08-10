using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Systems.Combat.Attacks.Item;

namespace NeoServer.Game.Systems.Combat.Systems;

public class RuneAttackSystem
{
    public bool Attack(IItem item, IPlayer aggressor, IThing enemy)
    {
        if (item is not IAttackRune rune) return false;

        var combatAttack = rune.NeedTarget ? RuneCombatAttack.Instance : AreaRuneCombatAttack.Instance;
        return rune.Use(aggressor, enemy, combatAttack);
    }
}
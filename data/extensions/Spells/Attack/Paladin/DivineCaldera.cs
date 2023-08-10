using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack.Paladin;

public class DivineCaldera: AttackSpell
{
    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);

        var magicLevel = player.MagicLevel;

        var min = player.MinimumAttackPower + (magicLevel * 5) + 25;
        var max = player.MinimumAttackPower + (magicLevel * 6.2) + 45;

        return new MinMax(min, (int)max);
    }

    public override ISpellCombatAttack CombatAttack => new SpellCombatAttack();
    public override byte Range => 1;
    public override bool NeedsTarget => false;
    public override DamageType DamageType => DamageType.Holy;
    public override string AreaName => "AREA_CIRCLE3X3";
}
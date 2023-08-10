using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack.Paladin;

public class EtherealSpear : AttackSpell
{
    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);

        var skillDistance = player.Skills[SkillType.Distance].Level;

        var min = (int)(player.MinimumAttackPower + skillDistance * 0.7);
        var max = player.MinimumAttackPower + skillDistance + 5;

        return new MinMax(min, max);
    }

    public override ISpellCombatAttack CombatAttack => new SpellCombatAttack();
    public override byte Range => 5;
    public override bool NeedsTarget => true;
    public override DamageType DamageType => DamageType.Physical;
    public override ShootType ShootType => ShootType.EtherealSpear;
}
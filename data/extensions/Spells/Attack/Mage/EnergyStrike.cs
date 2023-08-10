using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack.Mage;

public class EnergyStrike : AttackSpell
{
    public override DamageType DamageType => DamageType.Energy;
    public override ISpellCombatAttack CombatAttack { get; } = new SpellCombatAttack();
    public override byte Range => 5;

    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);

        var magicLevel = player.Skills[SkillType.Magic].Level;

        var min = player.MinimumAttackPower + magicLevel * 1.4 + 8;
        var max = player.MinimumAttackPower + magicLevel * 2.2 + 14;

        return new MinMax((int)min, (int)max);
    }
}
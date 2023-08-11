using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack;

public abstract class WaveSpell : AttackSpell
{
    public override ISpellCombatAttack CombatAttack => SpellCombatAttack.Instance;
    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);

        var magicLevel = player.Skills[SkillType.Magic].Level;

        var min = player.MinimumAttackPower + (magicLevel * 1.2) + 7;
        var max = player.MinimumAttackPower + (magicLevel * 2) + 12;

        return new MinMax((int)min, (int)max);
    }
}
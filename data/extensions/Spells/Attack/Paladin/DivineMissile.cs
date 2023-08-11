using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack.Paladin;

public class DivineMissile: AttackSpell
{
    public override DamageType DamageType => DamageType.Holy;
    public override ISpellCombatAttack CombatAttack => SpellCombatAttack.Instance;
    public override byte Range => 5;
    public override ShootType ShootType => ShootType.Holy;
    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);

        var magicLevel = player.MagicLevel;

        var min = player.MinimumAttackPower + (magicLevel * 1.9) + 8;
        var max = player.MinimumAttackPower + (magicLevel * 3) + 18;

        return new MinMax((int)min, (int)max);
    }
}
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack.Mage;

public class RageOfTheSkies : AttackSpell
{
    public override DamageType DamageType => DamageType.Energy;
    public override EffectT AreaEffect => EffectT.Energyarea;
    public override string AreaName => "AREA_CIRCLE6X6";
    public override ISpellCombatAttack CombatAttack => SpellCombatAttack.Instance;

    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);
        
        var magicLevel = player.MagicLevel;

        var min = player.MinimumAttackPower + magicLevel * 4 + 75;
        var max = player.MinimumAttackPower + magicLevel * 10 + 150;

        return new MinMax(min, max);
    }
}
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack;

public class Berserk : AttackSpell
{
    public override DamageType DamageType => DamageType.MagicalPhysical;
    public override string AreaName => "AREA_SQUARE3X3";
    public override ISpellCombatAttack CombatAttack { get; } = new SpellCombatAttack();
}
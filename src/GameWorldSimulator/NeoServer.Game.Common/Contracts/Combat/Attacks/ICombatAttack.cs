using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.Combat.Attacks;

public interface ICombatAttack
{
    bool TryAttack(ICombatActor aggressor, ICombatActor victim, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams);
}

public interface ICombatAttack2
{
    Result CauseDamage(IThing aggressor, IThing victim);
    CombatType CombatType { get; }
}

public interface IItemCombatAttack
{
    Result CauseDamage(IItem item, IThing aggressor, IThing victim);
}

public interface ISpellCombatAttack
{
    Result CauseDamage(IAttackSpell attackSpell, ICombatActor aggressor, ICombatActor victim);
    Result CanAttack(IAttackSpell attackSpell, ICombatActor aggressor);
}
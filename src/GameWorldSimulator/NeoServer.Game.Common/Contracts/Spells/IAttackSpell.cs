using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Spells;

public interface IAttackSpell: ISpell
{
    EffectT Effect { get; }
    uint Duration { get; }
    DamageType DamageType { get; }
    ConditionType ConditionType { get; }
    byte Range { get; }
    bool NeedsTarget { get; }
    ShootType ShootType { get; }
    CombatAttackParams PrepareAttack(ICombatActor actor);
    string AreaName { get; }
    bool OnCast(ICombatActor actor, string words, out InvalidOperation error);
}
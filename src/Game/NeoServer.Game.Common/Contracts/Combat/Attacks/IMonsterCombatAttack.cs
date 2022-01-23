using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Combat.Attacks;

public interface IMonsterCombatAttack
{
    byte Chance { get; set; }
    ICombatAttack CombatAttack { get; set; }
    DamageType DamageType { get; set; }
    int Interval { set; }
    byte Target { get; set; }

    CooldownTime Cooldown { get; }

    CombatAttackValue Translate();
}
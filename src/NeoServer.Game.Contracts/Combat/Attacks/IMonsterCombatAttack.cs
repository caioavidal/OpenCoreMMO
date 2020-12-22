using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Attacks
{
    public interface IMonsterCombatAttack
    {
        byte Chance {  get; set; }
        ICombatAttack CombatAttack {  get; set; }
        DamageType DamageType {  get; set; }
        int Interval { set; }
        bool IsMelee { get; }
        ushort MaxDamage {  get; set; }
        ushort MinDamage {  get; set; }
        byte Target {  get; set; }

        CooldownTime Cooldown { get; }

        CombatAttackValue Translate();
    }
}

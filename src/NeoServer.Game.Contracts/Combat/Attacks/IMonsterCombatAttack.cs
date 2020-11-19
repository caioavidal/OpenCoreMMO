using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Attacks
{
    public interface IMonsterCombatAttack
    {
        EffectT AreaEffect {  get; set; }
        byte Chance {  get; set; }
        ICombatAttack CombatAttack {  get; set; }
        DamageType DamageType {  get; set; }
        ushort Interval {  get; set; }
        bool IsMelee { get; }
        byte Length {  get; set; }
        ushort MaxDamage {  get; set; }
        ushort MinDamage {  get; set; }
        byte Radius {  get; set; }
        byte Spread {  get; set; }
        byte Target {  get; set; }

        CombatAttackValue Translate();
    }
}

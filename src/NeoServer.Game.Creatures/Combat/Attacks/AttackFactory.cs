using Microsoft.Extensions.ObjectPool;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class AttackFactory
    {

        //public static ICombatAttack Create(DamageType type, ushort attackPower, ushort minAttackPower, CombatAttackOption combatAttackOption)
        //{
        //    if(type == DamageType.Melee)
        //    {
        //        return new CombatAttack(DamageType.Melee);
        //    }
        //    if(type == DamageType.Physical)
        //    {
        //        return new DistanceCombatAttack();
        //    }
        //}
    }
}

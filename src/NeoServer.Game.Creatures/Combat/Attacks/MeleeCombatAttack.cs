using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class MeleeCombatAttack : CombatAttack, IMeleeCombatAttack
    {
        public MeleeCombatAttack(byte attack, byte skill) : base(DamageType.Melee, new CombatAttackOption() { Attack = attack, Skill = skill, Target = 1 })
        {
     
        }
        public byte Attack => Option.Attack;
        public byte Skill => Option.Skill;
    }
}

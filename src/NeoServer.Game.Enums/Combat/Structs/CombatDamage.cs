using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Combat.Structs
{
    public ref struct CombatDamage
    {
        public CombatDamage(ushort damage, DamageType type)
        {
            Damage = damage;
            Type = type;
            Missed = false;
        }
        public CombatDamage(ushort damage, DamageType type, bool missed)
        {
            Damage = damage;
            Type = type;
            Missed = missed;
        }

        /// <summary>
        /// Damage value to health or mana
        /// </summary>
        public ushort Damage { get; private set; }

        /// <summary>
        /// Type of the damage (physical, fire...)
        /// </summary>
        public DamageType Type { get; }

        /// <summary>
        /// true when distance missed damage
        /// </summary>
        public bool Missed { get; }


        /// <summary>
        /// Sets a new damage
        /// </summary>
        /// <param name="newDamage"></param>
        public void ReduceDamage(ushort newDamage)
        {
            Damage = newDamage;
        }

        /// <summary>
        /// Converts damage value to string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Damage.ToString();
    }
}

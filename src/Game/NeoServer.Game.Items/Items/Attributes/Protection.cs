using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Items.Attributes
{
    public sealed class Protection : IProtection
    {
        public Protection(Dictionary<DamageType, sbyte> damageProtection)
        {
            DamageProtection = damageProtection;
        }

        public Dictionary<DamageType, sbyte> DamageProtection { get; }

        private sbyte GetProtection(CombatDamage combatDamage)
        {
            var damageType = combatDamage.Type;
            if (DamageProtection is null || !DamageProtection.Any() || damageType == DamageType.None) return 0;
            
            if (DamageProtection.TryGetValue(damageType, out var value)) return value;

            if (damageType == DamageType.Melee)
            {
                if (DamageProtection.TryGetValue(DamageType.Physical, out var meleeProtection)) return meleeProtection;
            }

            if (combatDamage.IsElementalDamage)
            {
                if (DamageProtection.TryGetValue(DamageType.Elemental, out var elementalProtection)) return elementalProtection;
            }
            
            if (DamageProtection.TryGetValue(DamageType.All, out var protectionValue)) return protectionValue;

            return 0;
        }


        public bool Protect(ref CombatDamage damage)
        {
            var protection = GetProtection(damage);
            if (protection == 0) return false;
            damage.ReduceDamageByPercent(protection);
            return true;
        }
    }
}

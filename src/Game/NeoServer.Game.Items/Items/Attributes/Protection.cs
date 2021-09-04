using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Items.Attributes
{
    public sealed class Protection
    {
        public Protection(Dictionary<DamageType, byte> damageProtection)
        {
            DamageProtection = damageProtection;
        }

        public Dictionary<DamageType, byte> DamageProtection { get; }

        private byte GetProtection(DamageType damageType)
        {
            if (DamageProtection is null || damageType == DamageType.None) return 0;

            return !DamageProtection.TryGetValue(damageType, out var value) ? (byte)0 : value;
        }
        

        public void Protect(ref CombatDamage damage)
        {
            //if (NoCharges && !InfiniteCharges) return;

            var protection = GetProtection(damage.Type);
            if (protection == 0) return;
            damage.ReduceDamageByPercent(protection);
            //DecreaseCharges();
        }
    }
}

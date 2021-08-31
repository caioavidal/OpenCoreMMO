using System.Collections.Generic;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Protections
{
    public abstract class ProtectionItem : MoveableItem, IProtectionItem
    {
        protected ProtectionItem(IItemType type, Location location) : base(type, location)
        {
        }

        public Dictionary<DamageType, byte> DamageProtection => Metadata.Attributes.DamageProtection;

        public void DressedIn(IPlayer player)
        {
            player.OnAttacked += OnPlayerAttackedHandler;
        }

        public void UndressFrom(IPlayer player)
        {
            player.OnAttacked -= OnPlayerAttackedHandler;
        }

        public byte GetProtection(DamageType damageType)
        {
            if (DamageProtection is null || damageType == DamageType.None) return 0;

            return !DamageProtection.TryGetValue(damageType, out var value) ? (byte) 0 : value;
        }

        public virtual void OnPlayerAttackedHandler(IThing enemy, ICombatActor victim, ref CombatDamage damage)
        {
            Protect(ref damage);
        }

        public virtual void Protect(ref CombatDamage damage)
        {
            var protection = GetProtection(damage.Type);
            damage.ReduceDamageByPercent(protection);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public abstract class Accessory : MoveableItem, IProtectionItem, IChargeable, ISkillBonus//IDecayable, IDefenseEquipmentItem
    {
        protected Accessory(IItemType type, Location location) : base(type, location)
        {
            Charges = Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Charges);
        }

        #region Protection

        public Dictionary<DamageType, byte> DamageProtection => Metadata.Attributes.DamageProtection;
        
        public byte GetProtection(DamageType damageType)
        {
            if (DamageProtection is null || damageType == DamageType.None) return 0;

            return !DamageProtection.TryGetValue(damageType, out var value) ? (byte)0 : value;
        }

        public virtual void OnPlayerAttackedHandler(IThing enemy, ICombatActor victim, ref CombatDamage damage)
        {
            Protect(ref damage);
        }

        public virtual void Protect(ref CombatDamage damage)
        {
            if (NoCharges) return;

            var protection = GetProtection(damage.Type);
            if (protection == 0) return;
            damage.ReduceDamageByPercent(protection);
            DecreaseCharges();
        }

        #endregion

        #region Charges

        public ushort Charges { get; private set; }

        public bool NoCharges => Charges == 0;
        public void DecreaseCharges()
        {
            Charges--;
        }
        #endregion

        #region Skill Bonus
        public Dictionary<SkillType, byte> SkillBonuses => Metadata.Attributes.SkillBonuses;
        public void AddSkillBonus(IPlayer player)
        {
            if (Guard.AnyNull(SkillBonuses, player)) return;
            foreach (var (skillType, bonus) in SkillBonuses) player.AddSkillBonus(skillType, bonus);
        }

        public void RemoveSkillBonus(IPlayer player)
        {
            if (Guard.AnyNull(SkillBonuses, player)) return;
            foreach (var (skillType, bonus) in SkillBonuses) player.RemoveSkillBonus(skillType, bonus);
        }

        #endregion

        #region Dressable
        public void DressedIn(IPlayer player)
        {
            if (Guard.AnyNull(player)) return;
            player.OnAttacked += OnPlayerAttackedHandler;
            AddSkillBonus(player);
        }

        public void UndressFrom(IPlayer player)
        {
            if (Guard.AnyNull(player)) return;
            player.OnAttacked -= OnPlayerAttackedHandler;
            RemoveSkillBonus(player);
        }
        #endregion
    }
}

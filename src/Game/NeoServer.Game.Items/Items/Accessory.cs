using System.Collections.Generic;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Items
{
    public abstract class
        Accessory : MoveableItem, IProtectionItem, IChargeable, ISkillBonus, IDecayable //, IDefenseEquipmentItem
    {
        protected Accessory(IItemType type, Location location) : base(type, location)
        {
            Charges = Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Charges);
            Decayable = new Decayable(this, DecaysTo, Duration);
            Protection = new Protection(DamageProtection);
            SkillBonus = new SkillBonus(SkillBonuses);

            Decayable.OnDecayed += Decayed;
        }

        public Decayable Decayable { get; }
        public Protection Protection { get; }
        public SkillBonus SkillBonus { get; }
        public IPlayer PlayerDressing { get; private set; }

        private void Decayed(IDecayable item)
        {
            RemoveSkillBonus(PlayerDressing);
        }

        private void OnPlayerAttackedHandler(IThing enemy, ICombatActor victim, ref CombatDamage damage)
        {
            Protect(ref damage);
        }

        #region Protection

        public Dictionary<DamageType, byte> DamageProtection => Metadata.Attributes.DamageProtection;

        public void Protect(ref CombatDamage damage)
        {
            if (NoCharges && !InfiniteCharges) return;
            Protection.Protect(ref damage);
            DecreaseCharges();
        }

        #endregion

        #region Charges

        public ushort Charges { get; private set; }
        public bool NoCharges => Charges == 0;

        public void DecreaseCharges()
        {
            Charges -= (ushort) (Charges == 0 ? 0 : 1);
        }

        public bool InfiniteCharges => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Charges) == 0;

        #endregion

        #region Skill Bonus

        public Dictionary<SkillType, byte> SkillBonuses => Metadata.Attributes.SkillBonuses;

        public void AddSkillBonus(IPlayer player)
        {
            if (Decayable.Expired) return;
            SkillBonus.AddSkillBonus(player);
        }

        public void RemoveSkillBonus(IPlayer player)
        {
            SkillBonus.RemoveSkillBonus(player);
        }

        #endregion

        #region Dressable

        public void DressedIn(IPlayer player)
        {
            if (Guard.AnyNull(player)) return;
            player.OnAttacked += OnPlayerAttackedHandler;
            PlayerDressing = player;
            AddSkillBonus(player);
        }

        public void UndressFrom(IPlayer player)
        {
            if (Guard.AnyNull(player)) return;
            player.OnAttacked -= OnPlayerAttackedHandler;
            PlayerDressing = null;
            RemoveSkillBonus(player);
        }

        #endregion

        #region Decay

        public int DecaysTo => Metadata.Attributes.GetAttribute<int>(ItemAttribute.ExpireTarget);
        public int Duration => Metadata.Attributes.GetAttribute<int>(ItemAttribute.Duration);
        public bool ShouldDisappear => Decayable.ShouldDisappear;
        public bool Expired => Decayable.Expired;
        public int Elapsed => Decayable.Elapsed;

        public bool Decay()
        {
            return Decayable.Decay();
        }

        #endregion
    }
}
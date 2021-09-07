using System;
using System.Collections.Generic;
using System.Text;
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
    public abstract class Accessory : MoveableItem, IProtection, IChargeable, ISkillBonus, IDecayable, IDressable,
        ITransformable //, IDefenseEquipmentItem
    {
        protected Accessory(IItemType type, Location location) : base(type, location)
        {
            if(Duration > 0) Decayable = new Decayable(this, DecaysTo, Duration);

            Protection = new Protection(DamageProtection);
            SkillBonus = new SkillBonus(SkillBonuses);

            Charges = Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Charges);

            if(Decayable is not null) Decayable.OnDecayed += Decayed;

        }

        public IDecayable Decayable { get; }
        public IProtection Protection { get; }
        public ISkillBonus SkillBonus { get; }
        public IPlayer PlayerDressing { get; private set; }

        private void Decayed(IDecayable item, IItemType _)
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
            if (Expired) return;
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
            TransformOnEquip();
            StartDecay();
        }

        public void UndressFrom(IPlayer player)
        {
            if (Guard.AnyNull(player)) return;
            player.OnAttacked -= OnPlayerAttackedHandler;
            PlayerDressing = null;
            RemoveSkillBonus(player);
            TransformOnDequip();
            PauseDecay();
        }

        #endregion

        #region Decay

        public Func<IItemType> DecaysTo { get; init; }
        public int Duration => Metadata.Attributes.GetAttribute<int>(ItemAttribute.Duration);
        public bool ShouldDisappear => Decayable?.ShouldDisappear ?? false;
        public bool Expired => Decayable?.Expired ?? false;
        public int Elapsed => Decayable?.Elapsed ?? 0;
        public int Remaining => Decayable?.Remaining ?? default;

        public bool Decay()
        {
            return Decayable?.Decay() ?? default;
        }

        public event DecayDelegate OnDecayed;
        public event PauseDecay OnPaused;
        public event StartDecay OnStarted;
        public void StartDecay() => Decayable?.StartDecay();
        public void PauseDecay() => Decayable?.PauseDecay();

        #endregion

        #region Transformable

        public void TransformOnEquip()
        {
            if (TransformEquipItem?.Invoke() is not { } itemType) return;
            var before = Metadata;

            Metadata = itemType;
            OnTransformed?.Invoke(before, Metadata);
        }

        public void TransformOnDequip()
        {
            if (TransformDequipItem?.Invoke() is not { } itemType) return;

            var before = Metadata;
            Metadata = itemType;
            OnTransformed?.Invoke(before, Metadata);
        }

        public Func<IItemType> TransformEquipItem { get; init; }
        public Func<IItemType> TransformDequipItem { get; init; }
        public event Transform OnTransformed;

        #endregion

        public override string CustomLookText
        {
            get
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(base.CustomLookText);
                if (Decayable is not null)
                {
                    stringBuilder.Append($" that {Decayable}");
                }

                return stringBuilder.ToString();
            }

        }
    }
}
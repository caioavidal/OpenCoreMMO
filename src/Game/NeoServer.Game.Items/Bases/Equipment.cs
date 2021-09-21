using System;
using System.Text;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories.AttributeFactory;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Bases
{
    public abstract class Equipment : MoveableItem, IEquipment
    {
        
        protected Equipment(IItemType type, Location location) : base(type, location)
        {
            if (type.Attributes.SkillBonuses is not null) SkillBonus = new SkillBonus(this);
            Decayable = DecayableFactory.Create(this);
            Protection = ProtectionFactory.Create(this);

            if (Decayable is not null) Decayable.OnDecayed += Decayed;
        }

        public IDecayable Decayable { get; }
        public IProtection Protection { get; }
        public ISkillBonus SkillBonus { get; }
        public IChargeable Chargeable { get; init; }

        public IPlayer PlayerDressing { get; private set; }
        public Func<ushort, IItemType> ItemTypeFinder { get; init; }

        public override string CustomLookText
        {
            get
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(base.CustomLookText);
                if (Decayable is not null) stringBuilder.Append($" that {Decayable}");
                if (Chargeable is not null && Chargeable.ShowCharges) stringBuilder.Append($" that {Chargeable}");
                return stringBuilder.ToString();
            }
        }

        #region Protection

        public bool Protect(ref CombatDamage damage)
        {
            if (NoCharges) return false;
            var @protected = Protection?.Protect(ref damage) ?? false;
            if (@protected) DecreaseCharges();
            return true;
        }

        #endregion

        private void Decayed(ushort to)
        {
            var player = PlayerDressing;
            player.Inventory.RemoveItem(this, 1, (byte)Metadata.BodyPosition, out var removedThing);

            if (to == default)
            {
                return;
            }

            Metadata = ItemTypeFinder?.Invoke(to);
            if (Metadata is null) return;

            player.Inventory.AddItem(this, (byte)Metadata.BodyPosition);
        }

        private void Transformed(IItemType from, IItemType to)
        {
            Metadata = to;
            OnTransformed?.Invoke(from, to);
        }

        private void OnPlayerAttackedHandler(IThing enemy, ICombatActor victim, ref CombatDamage damage)
        {
            Protect(ref damage);
        }

        #region Charges

        public ushort Charges => Chargeable?.Charges ?? 0;
        public bool NoCharges => Chargeable?.NoCharges ?? false;
        public bool ShowCharges => Chargeable?.ShowCharges ?? false;
        public void DecreaseCharges() => Chargeable?.DecreaseCharges();
        #endregion

        #region Skill Bonus
        public void AddSkillBonus(IPlayer player)
        {
            if (Expired) return;
            SkillBonus?.AddSkillBonus(player);
        }

        public void RemoveSkillBonus(IPlayer player) => SkillBonus?.RemoveSkillBonus(player);

        #endregion

        #region Dressable

        public void DressedIn(IPlayer player)
        {
            
            if (Guard.AnyNull(player)) return;
            TransformOnEquip();

            player.OnAttacked += OnPlayerAttackedHandler;
            PlayerDressing = player;
            AddSkillBonus(player);
            StartDecay();
        }

        public void UndressFrom(IPlayer player)
        {
            if (Guard.AnyNull(player)) return;
            TransformOnDequip();

            player.OnAttacked -= OnPlayerAttackedHandler;
            PlayerDressing = null;
            RemoveSkillBonus(player);
            PauseDecay();
        }

        #endregion

        #region Decay

        public ushort DecaysTo => Decayable?.DecaysTo ?? default;
        public uint Duration => Decayable?.Duration ?? 0;
        public bool ShouldDisappear => Decayable?.ShouldDisappear ?? false;
        public bool Expired => Decayable?.Expired ?? false;
        public uint Elapsed => Decayable?.Elapsed ?? 0;
        public uint Remaining => Decayable?.Remaining ?? default;

        public bool TryDecay()
        {
            return Decayable?.TryDecay() ?? default;
        }

        public event DecayDelegate OnDecayed;
        public event PauseDecay OnPaused;
        public event StartDecay OnStarted;

        public void StartDecay()
        {
            if (Guard.AnyNull(Metadata)) return;
            if (Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.StopDecaying, out var stopDecaying) && stopDecaying == 1) return;
            Decayable?.StartDecay();
        }

        public void PauseDecay()
        {
            if (Metadata is null) return;

            var hasStopDecaying = Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.StopDecaying, out var stopDecaying);
            if (!hasStopDecaying || stopDecaying == 0) return;

            Decayable?.PauseDecay();
        }
        #endregion

        #region Transformable

        public void TransformOnEquip()
        {
            if (!Metadata.Attributes.HasAttribute(ItemAttribute.TransformEquipTo)) return;
            var before = Metadata;
            Metadata = TransformEquipItem;
            OnTransformed?.Invoke(before, Metadata);
        }

        public void TransformOnDequip()
        {
            if (!Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.TransformDequipTo, out _)) return;
            var before = Metadata;
            Metadata = TransformDequipItem;
            OnTransformed?.Invoke(before, Metadata);
        }

        public IItemType TransformEquipItem => ItemTypeFinder?.Invoke(Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.TransformEquipTo));
        public IItemType TransformDequipItem => ItemTypeFinder?.Invoke(Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.TransformDequipTo));
        public event Transform OnTransformed;

        #endregion
    }
}
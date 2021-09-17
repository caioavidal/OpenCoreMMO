using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Bases
{
    public abstract class Equipment : MoveableItem, IEquipment
    {
        private readonly IDecayable _decayable;
        private readonly ITransformable _transformable;

        protected Equipment(IItemType type, Location location) : base(type, location)
        {
        }

        public IDecayable Decayable
        {
            get => _decayable;
            init
            {
                if (value is null) return;
                _decayable = value;
                Decayable.OnDecayed += Decayed;
            }
        }

        public IProtection Protection { get; init; }
        public ISkillBonus SkillBonus { get; init; }
        public IChargeable Chargeable { get; init; }

        public ITransformable Transformable
        {
            get => _transformable;
            init
            {
                if (value is null) return;
                _transformable = value;
                _transformable.OnTransformed += Transformed;
            }
        }

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

        private void Decayed(IItemType to)
        {
            if (to is null)
                PlayerDressing.Inventory.RemoveItem(this, 1, (byte)Metadata.BodyPosition, out var removedThing);
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

        public void RemoveSkillBonus(IPlayer player)
        {
            SkillBonus?.RemoveSkillBonus(player);
        }

        public void ChangeSkillBonuses(Dictionary<SkillType, byte> skillBonuses) => SkillBonus?.ChangeSkillBonuses(skillBonuses);
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

        public Func<IItemType> DecaysTo => Decayable?.DecaysTo;
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
            if (Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.StopDecaying, out var stopDecaying) && stopDecaying == 1) return;
            Decayable?.StartDecay();
        }

        public void PauseDecay()
        {
            var hasStopDecaying = Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.StopDecaying, out var stopDecaying);
            if (!hasStopDecaying || hasStopDecaying && stopDecaying == 0) return;

            Decayable?.PauseDecay();
        }

        public void SetDuration(uint duration) => Decayable?.SetDuration(duration);

        #endregion

        #region Transformable

        public void TransformOnEquip()
        {
            Transformable?.TransformOnEquip();
            ChangeSkillBonuses(Metadata.Attributes.SkillBonuses);
            SetDuration(Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Duration));
        }

        public void TransformOnDequip() => Transformable?.TransformOnDequip();
        public Func<IItemType> TransformEquipItem => Transformable?.TransformEquipItem;
        public Func<IItemType> TransformDequipItem => Transformable?.TransformDequipItem;
        public event Transform OnTransformed;

        #endregion
    }
}
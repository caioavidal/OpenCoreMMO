using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Model.Players
{
    public class PlayerInventory : IInventory
    {

        public event AddItemToSlot OnItemAddedToSlot;
        public event FailAddItemToSlot OnFailedToAddToSlot;
        private IDictionary<Slot, Tuple<IPickupable, ushort>> Inventory { get; }

        public ushort TotalAttack
        {
            get
            {
                ushort attack = 0;

                if (Weapon is IWeaponItem weapon) return weapon.Attack;

                if (Weapon is IDistanceWeaponItem distance)
                {
                    attack += distance.ExtraAttack;
                    if (Ammo != null)
                    {
                        attack += distance.ExtraAttack;
                    }

                }

                return attack;
            }
        }

        public IAmmoItem Ammo => Inventory.ContainsKey(Slot.Ammo) && Inventory[Slot.Ammo].Item1 is IAmmoItem ammo ? Inventory[Slot.Ammo].Item1 as IAmmoItem : null;

        public ushort TotalDefense
        {
            get
            {
                var totalDefense = 0;
                if (Weapon is IWeaponItem weapon)
                {
                    totalDefense += weapon.Defense;
                }

                totalDefense += Shield?.DefenseValue ?? 0;

                return (ushort)totalDefense;
            }
        }

        public IDefenseEquipmentItem Shield => Inventory.ContainsKey(Slot.Right) ? Inventory[Slot.Right].Item1 as IDefenseEquipmentItem : null;

        public IWeapon Weapon => Inventory.ContainsKey(Slot.Left) ? Inventory[Slot.Left].Item1 as IWeapon : null;

        public byte TotalArmor
        {
            get
            {
                byte totalArmor = 0;

                Func<Slot, ushort> getDefenseValue = (Slot slot) => (Inventory[Slot.Head].Item1 as IDefenseEquipmentItem).DefenseValue;

                totalArmor += (byte)(Inventory.ContainsKey(Slot.Necklace) ? getDefenseValue(Slot.Necklace) : 0);
                totalArmor += (byte)(Inventory.ContainsKey(Slot.Head) ? getDefenseValue(Slot.Head) : 0);
                totalArmor += (byte)(Inventory.ContainsKey(Slot.Body) ? getDefenseValue(Slot.Body) : 0);
                totalArmor += (byte)(Inventory.ContainsKey(Slot.Legs) ? getDefenseValue(Slot.Legs) : 0);
                totalArmor += (byte)(Inventory.ContainsKey(Slot.Feet) ? getDefenseValue(Slot.Feet) : 0);
                totalArmor += (byte)(Inventory.ContainsKey(Slot.Ring) ? getDefenseValue(Slot.Ring) : 0);

                return totalArmor;
            }
        }

        public byte AttackRange
        {
            get
            {
                var rangeLeft = 0;
                var rangeRight = 0;
                var twoHanded = 0;

                if (Inventory.ContainsKey(Slot.Left) && Inventory[Slot.Left] is IAmmoItem leftWeapon)
                {
                    rangeLeft = leftWeapon.Range;
                }
                if (Inventory.ContainsKey(Slot.Right) && Inventory[Slot.Right] is IAmmoItem rightWeapon)
                {
                    rangeRight = rightWeapon.Range;
                }
                if (Inventory.ContainsKey(Slot.TwoHanded) && Inventory[Slot.TwoHanded] is IAmmoItem twoHandedWeapon)
                {
                    rangeRight = twoHandedWeapon.Range;
                }

                return (byte)Math.Max(Math.Max(rangeLeft, rangeRight), twoHanded);
            }
        }

        public IPlayer Owner { get; }

        public PlayerInventory(IPlayer owner, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            Owner = owner;

            Inventory = inventory ?? new Dictionary<Slot, Tuple<IPickupable, ushort>>();
        }

        public IItem this[Slot slot] => !Inventory.ContainsKey(slot) ? null : Inventory[slot].Item1;

        public IContainer BackpackSlot => this[Slot.Backpack] is IContainer container ? container : null;

        public float TotalWeight
        {
            get
            {
                var sum = 0F;
                foreach (var item in Inventory.Values)
                {
                    sum += item.Item1.Weight;
                }
                return sum;
            }
        }

        public Result<IPickupable> TryAddItemToSlot(Slot slot, IPickupable item)
        {
            bool canCarry = CanCarryItem(item, slot);

            if (!canCarry)
            {
                OnFailedToAddToSlot?.Invoke(InvalidOperation.TooHeavy);
                return new Result<IPickupable>(InvalidOperation.TooHeavy);
            }

            var canAddItemToSlot = CanAddItemToSlot(slot, item);
            if (!canAddItemToSlot.Value)
            {
                OnFailedToAddToSlot?.Invoke(canAddItemToSlot.Error);
                return new Result<IPickupable>(canAddItemToSlot.Error);
            }

            if (slot == Slot.Backpack)
            {
                if (Inventory.ContainsKey(Slot.Backpack))
                {

                    return new Result<IPickupable>(null, (Inventory[slot].Item1 as IPickupableContainer).TryAddItem(item).Error);
                }
                else if (item is IPickupableContainer container)
                {
                    container.SetParent(Owner);
                }
            }
            var slotHasItem = Inventory.ContainsKey(slot);

            //todo: refact
            if (slotHasItem)
            {
                Tuple<IPickupable, ushort> itemToSwap = null;

                if (item is ICumulativeItem cumulative)
                {
                    if (NeedToSwap(cumulative, slot))
                    {
                        itemToSwap = SwapItem(slot, cumulative);
                    }
                    else
                    {
                        (Inventory[slot].Item1 as ICumulativeItem).TryJoin(ref cumulative);
                        if (cumulative?.Amount > 0)
                        {
                            itemToSwap = new Tuple<IPickupable, ushort>(cumulative, cumulative.ClientId);
                        }
                    }
                }
                else
                {
                    itemToSwap = SwapItem(slot, item);
                }

                OnItemAddedToSlot?.Invoke(this, item, slot);
                return itemToSwap == null ? new Result<IPickupable>() : new Result<IPickupable>(itemToSwap.Item1);
            }

            Inventory.Add(slot, new Tuple<IPickupable, ushort>(item, item.ClientId));

            OnItemAddedToSlot?.Invoke(this, item, slot);
            return new Result<IPickupable>();
        }

        private Tuple<IPickupable, ushort> SwapItem(Slot slot, IPickupable item)
        {
            var itemToSwap = Inventory[slot];
            Inventory[slot] = new Tuple<IPickupable, ushort>(item, item.ClientId);
            return itemToSwap;
        }

        private bool NeedToSwap(IPickupable itemToAdd, Slot slotDestination)
        {
            if (!Inventory.ContainsKey(slotDestination))
            {
                return false;
            }

            var itemOnSlot = Inventory[slotDestination].Item1;

            if (itemToAdd is ICumulativeItem cumulative && itemOnSlot.ClientId == cumulative.ClientId)
            {
                //will join
                return false;
            }

            if (slotDestination == Slot.Backpack)
            {
                // will add item to container
                return false;
            }

            return true;
        }

        private bool CanCarryItem(IPickupable item, Slot slot)
        {

            if (NeedToSwap(item, slot))
            {
                var itemOnSlot = Inventory[slot].Item1;

                return (TotalWeight - itemOnSlot.Weight + item.Weight) <= Owner.CarryStrength;
            }

            float weight = item.Weight;

            if (item is ICumulativeItem cumulative && slot == Slot.Ammo)
            {
                byte amountToAdd = cumulative.Amount > cumulative.AmountToComplete ? cumulative.AmountToComplete : cumulative.Amount;
                weight = cumulative.CalculateWeight(amountToAdd);
            }

            var canCarry = (TotalWeight + weight) <= Owner.CarryStrength;
            return canCarry;
        }

        private Result<bool> CanAddItemToSlot(Slot slot, IItem item)
        {
            var cannotDressFail = new Result<bool>(false, InvalidOperation.CannotDress);

            if (slot == Slot.Backpack)
            {
                if (item is IPickupableContainer && !Inventory.ContainsKey(Slot.Backpack))
                {
                    return new Result<bool>(true);
                }
                return Inventory.ContainsKey(Slot.Backpack) ? new Result<bool>(true) : cannotDressFail;
            }

            if (!(item is IInventoryItem inventoryItem))
            {
                return cannotDressFail;
            }

            if (inventoryItem is IWeapon weapon)
            {
                if (slot != Slot.Left)
                {
                    return cannotDressFail;
                }

                var hasShieldDressed = this[Slot.Right] != null;

                if (weapon.TwoHanded && hasShieldDressed)
                {
                    //trying to add a two handed while right slot has a shield
                    return new Result<bool>(false, InvalidOperation.BothHandsNeedToBeFree);
                }

                return new Result<bool>(true);
            }

            if (slot == Slot.Right && this[Slot.Left] is IWeapon weaponOnLeft && weaponOnLeft.TwoHanded)
            {
                //trying to add a shield while left slot has a two handed weapon
                return new Result<bool>(false, InvalidOperation.BothHandsNeedToBeFree);
            }

            if (inventoryItem.Slot != slot)
            {
                return cannotDressFail;
            }

            return new Result<bool>(true);
        }

    }
}

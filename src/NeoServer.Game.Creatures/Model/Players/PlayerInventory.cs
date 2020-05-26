using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Model.Players
{
    public class PlayerInventory : IInventory
    {
        private IDictionary<Slot, Tuple<IPickupable, ushort>> Inventory { get; }

        public byte TotalAttack => (byte)Math.Max(Inventory.ContainsKey(Slot.Left) ? (Inventory[Slot.Left].Item1 as IWeaponItem).Attack : 0, Inventory.ContainsKey(Slot.Right) ? (Inventory[Slot.Right].Item1 as IWeaponItem).Attack : 0);

        public byte TotalDefense => (byte)Math.Max(Inventory.ContainsKey(Slot.Left) ? (Inventory[Slot.Left].Item1 as IWeaponItem).Defense : 0, Inventory.ContainsKey(Slot.Right) ? (Inventory[Slot.Right].Item1 as IWeaponItem).Defense : 0);

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


        public Result TryAddItemToSlot(Slot slot, IPickupable item)
        {
            var canCarry = (TotalWeight + item.Weight) <= Owner.CarryStrength;

            if (!canCarry)
            {
                return new Result(InvalidOperation.TooHeavy);
            }

            var canAddItemToSlot = CanAddItemToSlot(slot, item);
            if (!canAddItemToSlot.Value)
            {
                return new Result(canAddItemToSlot.Error);
            }

            if (slot == Slot.Backpack)
            {
                if (Inventory.ContainsKey(Slot.Backpack))
                {
                    return (Inventory[slot].Item1 as IPickupableContainer).TryAddItem(item);
                }
                else if (item is IPickupableContainer container)
                {
                    container.SetParent(Owner);
                }
            }

            if (Inventory.ContainsKey(slot))
            {
                Inventory[slot] = new Tuple<IPickupable, ushort>(item, item.ClientId);
            }

            Inventory.Add(slot, new Tuple<IPickupable, ushort>(item, item.ClientId));

            return new Result();
        }

        private Result<bool> CanAddItemToSlot(Slot slot, IItem item)
        {
            var cannotDressFail = new Result<bool>(false, InvalidOperation.CannotDress);

            if (!(item is IInventoryItem inventoryItem))
            {
                return cannotDressFail;
            }

            if (inventoryItem is IWeapon weapon)
            {
                if(slot != Slot.Left)
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

            if (inventoryItem.Slot != slot)
            {
                if( slot == Slot.Backpack && Inventory.ContainsKey(Slot.Backpack))
                {
                    return new Result<bool>(true);

                }
                return cannotDressFail;
            }
            if (slot == Slot.Right && this[Slot.Left] is IWeapon weaponOnLeft && weaponOnLeft.TwoHanded)
            {
                //trying to add a shield while left slot has a two handed weapon
                return new Result<bool>(false, InvalidOperation.BothHandsNeedToBeFree);
            }
           

            return new Result<bool>(true);
        }

    }
}

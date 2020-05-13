using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Model.Players
{
    public class PlayerInventory : IInventory
    {
        private IDictionary<Slot, Tuple<IItem, ushort>> Inventory { get; }

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


        public ICreature Owner { get; }

        public PlayerInventory(ICreature owner, IDictionary<Slot, Tuple<IItem, ushort>> inventory)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            Owner = owner;

            Inventory = inventory ?? new Dictionary<Slot, Tuple<IItem, ushort>>();
        }

        public IItem this[Slot slot] => !Inventory.ContainsKey(slot) ? null : Inventory[slot].Item1;

        public IContainer BackpackSlot => this[Slot.Backpack] is IContainer container ? container : null;

        public bool TryAddItemToSlot(Slot slot, IItem item)
        {

            if(!(item is IInventoryItem inventoryItem))
            {
                return false;
            }
            if (inventoryItem.Slot != slot) 
            {
                return false;
            }

            if (Inventory.ContainsKey(slot))
            {
                Inventory[slot] = new Tuple<IItem, ushort>(item, item.ClientId);
            }

            Inventory.Add(slot, new Tuple<IItem, ushort>(item, item.ClientId));
            return true;
        }
    }
}

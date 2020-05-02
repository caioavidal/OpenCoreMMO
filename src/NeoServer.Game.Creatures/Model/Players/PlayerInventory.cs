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

                if (Inventory.ContainsKey(Slot.Left) && Inventory[Slot.Left] is IDistanceWeaponItem leftWeapon)
                {
                    rangeLeft = leftWeapon.MaxAttackDistance;
                }
                if (Inventory.ContainsKey(Slot.Right) && Inventory[Slot.Right] is IDistanceWeaponItem rightWeapon)
                {
                    rangeRight = rightWeapon.MaxAttackDistance;
                }
                if (Inventory.ContainsKey(Slot.TwoHanded) && Inventory[Slot.TwoHanded] is IDistanceWeaponItem twoHandedWeapon)
                {
                    rangeRight = twoHandedWeapon.MaxAttackDistance;
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

        public bool Add(IItem item, out IItem extraItem, byte positionByte, byte count = 1, ushort lossProbability = 300)
        {
            throw new NotImplementedException(); //todo

            //if (count == 0 || count > 100)
            //{
            //    throw new ArgumentException($"Invalid count {count}.", nameof(count));
            //}

            //extraItem = null;

            //if (item == null || positionByte > (byte)Slot.WhereEver)
            //{
            //    return false;
            //}

            //var targetSlot = (Slot)positionByte;

            //// TODO: check dress positions here.

            //// if (targetSlot != Slot.Right && targetSlot != Slot.Left && targetSlot != Slot.WhereEver)
            //// {

            //// }
            //try
            //{
            //    var current = Inventory[targetSlot];

            //    if (current != null)
            //    {
            //        var joinResult = current.Item1.Join(item);

            //        // update the added item in the slot.
            //        Game.Instance.NotifySinglePlayer(Owner as IPlayer, conn => new GenericNotification(conn, new InventorySetSlotPacket { Slot = targetSlot, Item = current.Item1 }));

            //        if (joinResult || current.Item1.IsContainer)
            //        {
            //            return joinResult;
            //        }

            //        // exchange items
            //        if (current.Item1.IsCumulative && item.Type.TypeId == current.Item1.Type.TypeId && current.Item1.Count == 100)
            //        {
            //            extraItem = item;
            //            item = current.Item1;
            //        }
            //        else
            //        {
            //            extraItem = current.Item1;
            //            extraItem.SetHolder(null, default(Location));
            //        }
            //    }
            //}
            //catch
            //{
            //    // ignored
            //}

            //// set the item in place.
            //Inventory[targetSlot] = new Tuple<IItem, ushort>(item, item.IsContainer ? (ushort)1000 : lossProbability);

            //item.SetHolder(Owner, new Location { X = 0xFFFF, Y = 0, Z = (sbyte)targetSlot });

            //// update the added item in the slot.
            //Game.Instance.NotifySinglePlayer(Owner as IPlayer, conn => new GenericNotification(conn, new InventorySetSlotPacket { Slot = targetSlot, Item = item }));

            //return true;
        }

        public IItem Remove(byte positionByte, byte count, out bool wasPartial)
        {
            throw new NotImplementedException(); //todo

            //wasPartial = false;

            //if (positionByte == (byte)Slot.TwoHanded || positionByte == (byte)Slot.WhereEver)
            //{
            //	return null;
            //}

            //if (Inventory.ContainsKey((Slot)positionByte))
            //{
            //	var found = Inventory[(Slot)positionByte].Item1;

            //	if (found.Count < count)
            //	{
            //		return null;
            //	}

            //	// remove the whole item
            //	if (found.Count == count)
            //	{
            //		Inventory.Remove((Slot)positionByte);
            //		found.SetHolder(null, default(Location));

            //		// update the slot.
            //		Game.Instance.NotifySinglePlayer(
            //			Owner as IPlayer,
            //			conn => new GenericNotification(
            //				conn,
            //				new InventoryClearSlotPacket { Slot = (Slot)positionByte }));

            //		return found;
            //	}

            //	IItem newItem = ItemFactory.Create(found.Type.TypeId);

            //	newItem.SetAmount(count);
            //	found.SetAmount((byte)(found.Amount - count));

            //	// update the remaining item in the slot.
            //	Game.Instance.NotifySinglePlayer(
            //		Owner as IPlayer,
            //		conn => new GenericNotification(
            //			conn,
            //			new InventorySetSlotPacket { Slot = (Slot)positionByte, Item = found }));

            //	wasPartial = true;
            //	return newItem;
            //}

            //return null;
        }

        public IItem Remove(ushort itemId, byte count, out bool wasPartial)
        {
            throw new NotImplementedException(); //todo
                                                 //wasPartial = false;

            //var slot = Inventory.Keys.FirstOrDefault(k => Inventory[k].Item1.Type.TypeId == itemId);

            //if (slot != default(Slot))
            //{
            //	var found = Inventory[slot].Item1;

            //	if (found.Count < count)
            //	{
            //		return null;
            //	}

            //	// remove the whole item
            //	if (found.Count == count)
            //	{
            //		Inventory.Remove(slot);
            //		found.SetHolder(null, default(Location));

            //		// update the slot.
            //		Game.Instance.NotifySinglePlayer(
            //			Owner as IPlayer,
            //			conn => new GenericNotification(
            //				conn,
            //				new InventoryClearSlotPacket { Slot = slot }));

            //		return found;
            //	}

            //	IItem newItem = ItemFactory.Create(found.Type.TypeId);

            //	newItem.SetAmount(count);
            //	found.SetAmount((byte)(found.Amount - count));

            //	// update the remaining item in the slot.
            //	Game.Instance.NotifySinglePlayer(
            //		Owner as IPlayer,
            //		conn => new GenericNotification(
            //			conn,
            //			new InventorySetSlotPacket { Slot = slot, Item = found }));

            //	wasPartial = true;
            //	return newItem;
            //}

            //// TODO: exhaustive search of container items here.
            //return null;
        }

    }
}

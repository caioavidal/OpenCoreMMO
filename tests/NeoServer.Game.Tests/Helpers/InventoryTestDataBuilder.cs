using System;
using System.Collections.Generic;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Creatures.Model.Players;

namespace NeoServer.Game.Tests.Helpers
{
    public static class InventoryTestDataBuilder
    {
        public static IInventory Build(IPlayer player = null, Dictionary<Slot, Tuple<IPickupable, ushort>> inventoryMap = null, ICoinTypeStore coinTypeStore = null)
        {
            player ??= PlayerTestDataBuilder.Build();
            inventoryMap ??= new Dictionary<Slot, Tuple<IPickupable, ushort>>();
            
            return new Inventory(player, inventoryMap);
        }
        
        public static Dictionary<Slot, Tuple<IPickupable, ushort>> GenerateInventory() =>
            new()
            {
                [Slot.Backpack] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBackpack(), 1),
                [Slot.Ammo] = new Tuple<IPickupable, ushort>(ItemTestData.CreateAmmo(2, 10), 2),
                [Slot.Head] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(3, "head"), 3),
                [Slot.Left] = new Tuple<IPickupable, ushort>(ItemTestData.CreateWeaponItem(4, "axe"), 4),
                [Slot.Body] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(5, "body"), 5),
                [Slot.Feet] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(6, "feet"), 6),
                [Slot.Right] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(7, "", "shield"), 7),
                [Slot.Ring] =
                    new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(id: 8, slot: "ring"), 8),
                [Slot.Necklace] =
                    new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(id: 10, slot: "necklace"),
                        10),
                [Slot.Legs] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(11, "legs"), 11)
            };
    }
}
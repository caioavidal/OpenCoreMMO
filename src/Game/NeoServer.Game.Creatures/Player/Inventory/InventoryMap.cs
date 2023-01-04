using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Creatures.Player.Inventory;

internal class InventoryMap
{
    private Inventory Inventory { get; }

    public InventoryMap(Inventory inventory)
    {
        Inventory = inventory;
        _map = new Dictionary<Slot, (IPickupable, ushort)>();
    }

    private readonly IDictionary<Slot, (IPickupable, ushort)> _map;
    internal IDictionary<ushort, uint> Map
    {
        get
        {
            var map = Inventory.BackpackSlot?.Map ?? new Dictionary<ushort, uint>();

            void AddOrUpdate(IItem item)
            {
                if (item is null) return;
                if (map.TryGetValue(item.Metadata.TypeId, out var val))
                    map[item.Metadata.TypeId] = val + item.Amount;
                else
                    map.Add(item.Metadata.TypeId, item.Amount);
            }

            AddOrUpdate(Inventory[Slot.Head]);
            AddOrUpdate(Inventory[Slot.Necklace]);
            AddOrUpdate(Inventory[Slot.Body]);
            AddOrUpdate(Inventory[Slot.Right]);
            AddOrUpdate(Inventory[Slot.Left]);
            AddOrUpdate(Inventory[Slot.Legs]);
            AddOrUpdate(Inventory[Slot.Ring]);
            AddOrUpdate(Inventory[Slot.Ammo]);

            return map;
        }
    }

    internal IEnumerable<(IPickupable, ushort)> Items => _map.Values;
    internal T GetItem<T>(Slot slot) =>
        _map.ContainsKey(slot) && _map[slot].Item1 is T item
            ? item
            : default;

    internal (IPickupable, ushort) GetItem(Slot slot) =>
        _map.TryGetValue(slot, out var item) ? item : default;

    internal bool HasItemOnSlot(Slot slot) => _map.TryGetValue(slot, out var item) && item.Item1 is not null;

    internal void Remove(Slot slot) => _map.Remove(slot);
    internal void Add(Slot slot, IPickupable item, ushort itemId) => _map.TryAdd(slot, (item, itemId));
    internal void Update(Slot slot, IPickupable item, ushort itemId) => _map[slot] = (item, itemId);


}
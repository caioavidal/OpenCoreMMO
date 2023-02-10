using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.Creatures;

public delegate void RemoveItemFromSlot(IInventory inventory, IPickupable item, Slot slot, byte amount = 1);

public delegate void AddItemToSlot(IInventory inventory, IPickupable item, Slot slot, byte amount = 1);

public delegate void FailAddItemToSlot(IPlayer player, InvalidOperation invalidOperation);

public interface IInventory : IHasItem
{
    IPlayer Owner { get; }
    ushort TotalAttack { get; }

    ushort TotalDefense { get; }

    ushort TotalArmor { get; }

    byte AttackRange { get; }
    IContainer BackpackSlot { get; }
    IWeapon Weapon { get; }
    bool HasShield { get; }
    float TotalWeight { get; }
    IDictionary<ushort, uint> Map { get; }
    IEnumerable<IItem> DressingItems { get; }
    bool IsUsingWeapon { get; }
    IItem this[Slot slot] { get; }
    ulong GetTotalMoney(ICoinTypeStore coinTypeStore);
    event AddItemToSlot OnItemAddedToSlot;
    event FailAddItemToSlot OnFailedToAddToSlot;
    event RemoveItemFromSlot OnItemRemovedFromSlot;

    Result<IPickupable> RemoveItem(Slot slot, byte amount);
    T TryGetItem<T>(Slot slot);
    Result<OperationResultList<IItem>> AddItem(IItem thing, Slot slot = Slot.None);
}
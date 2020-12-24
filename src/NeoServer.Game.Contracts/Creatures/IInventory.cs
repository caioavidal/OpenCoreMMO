using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void RemoveItemFromSlot(IInventory inventory, IPickupable item, Slot slot, byte amount = 1);
    public delegate void AddItemToSlot(IInventory inventory, IPickupable item, Slot slot, byte amount = 1);
    public delegate void FailAddItemToSlot(InvalidOperation invalidOperation);
    public interface IInventory: IStore
    {
        IPlayer Owner { get; }

        ushort TotalAttack { get; }

        ushort TotalDefense { get; }

        byte TotalArmor { get; }

        byte AttackRange { get; }
        Items.Types.IContainer BackpackSlot { get; }
        IWeapon Weapon { get; }
        bool HasShield { get; }
        float TotalWeight { get; }

        IItem this[Slot slot] { get; }

        event AddItemToSlot OnItemAddedToSlot;
        event FailAddItemToSlot OnFailedToAddToSlot;
        event RemoveItemFromSlot OnItemRemovedFromSlot;

        Result<IPickupable> TryAddItemToSlot(Slot slot, IPickupable item);
        bool RemoveItemFromSlot(Slot slot, byte amount, out IPickupable removedItem);
        Result<bool> CanAddItemToSlot(Slot slot, IItem item);
    }
}

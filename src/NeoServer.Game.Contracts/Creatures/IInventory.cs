using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void AddItemToSlot(IInventory inventory, IPickupable item, Slot slot, byte amount = 1);
    public delegate void FailAddItemToSlot(InvalidOperation invalidOperation);
    public interface IInventory
    {
        IPlayer Owner { get; }

        ushort TotalAttack { get; }

        byte TotalDefense { get; }

        byte TotalArmor { get; }

        byte AttackRange { get; }
        Items.Types.IContainer BackpackSlot { get; }
        IWeapon Weapon { get; }

        IItem this[Slot slot] { get; }

        event AddItemToSlot OnItemAddedToSlot;
        event FailAddItemToSlot OnFailedToAddToSlot;

        Result<IPickupable> TryAddItemToSlot(Slot slot, IPickupable item);
    }
}

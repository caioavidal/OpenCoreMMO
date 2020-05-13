using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Players;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IInventory
    {
        ICreature Owner { get; }

        byte TotalAttack { get; }

        byte TotalDefense { get; }

        byte TotalArmor { get; }

        byte AttackRange { get; }
        Items.Types.IContainer BackpackSlot { get; }

        IItem this[Slot slot] { get; }
    }
}

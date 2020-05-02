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

        bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300);

        IItem Remove(byte positionByte, byte count, out bool wasPartial);

        IItem Remove(ushort itemId, byte count, out bool wasPartial);
    }
}

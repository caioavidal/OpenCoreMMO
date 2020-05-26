using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IInventory
    {
        IPlayer Owner { get; }

        byte TotalAttack { get; }

        byte TotalDefense { get; }

        byte TotalArmor { get; }

        byte AttackRange { get; }
        Items.Types.IContainer BackpackSlot { get; }

        IItem this[Slot slot] { get; }
    }
}

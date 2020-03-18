using NeoServer.Server.Model.Items.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Creatures.Contracts
{
    public interface IInventory
    {
        ICreature Owner { get; }

        byte TotalAttack { get; }

        byte TotalDefense { get; }

        byte TotalArmor { get; }

        byte AttackRange { get; }

        IItem this[byte idx] { get; }

        bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300);

        IItem Remove(byte positionByte, byte count, out bool wasPartial);

        IItem Remove(ushort itemId, byte count, out bool wasPartial);
    }
}

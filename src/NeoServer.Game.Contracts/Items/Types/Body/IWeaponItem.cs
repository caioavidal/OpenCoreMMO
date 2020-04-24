using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IWeaponItem : IBodyEquipmentItem
    {
        ushort Attack { get; }
        byte Defense { get; }

        Tuple<DamageType, byte> ElementalDamage { get; }
        sbyte ExtraDefense { get; }
        bool TwoHanded { get; }

    }
}

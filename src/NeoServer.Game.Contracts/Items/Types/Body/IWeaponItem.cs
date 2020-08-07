using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Players;
using System;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IWeapon: IBodyEquipmentItem
    {
        bool TwoHanded => Metadata.BodyPosition == Slot.TwoHanded;

        Slot Slot => Slot.Left;
        public WeaponType Type => Metadata.WeaponType;


    }
    public interface IWeaponItem : IWeapon, IBodyEquipmentItem
    {
        ushort Attack { get; }
        byte Defense { get; }

        Tuple<DamageType, byte> ElementalDamage { get; }
        sbyte ExtraDefense { get; }
    }
}

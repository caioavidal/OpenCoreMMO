using System;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body;

public interface IAmmoEquipment : ICumulative, IBodyEquipmentEquipment
{
    byte Attack { get; }
    byte ExtraHitChance { get; }
    AmmoType AmmoType { get; }
    ShootType ShootType { get; }
    bool HasElementalDamage { get; }
    Tuple<DamageType, byte> ElementalDamage { get; }

    void Throw();
}
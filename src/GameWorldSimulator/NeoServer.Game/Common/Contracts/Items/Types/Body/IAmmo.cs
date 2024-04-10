using NeoServer.Game.Common.Item;
using NeoServer.Game.Item.Items.Weapons;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body;

public interface IAmmo : ICumulative, IBodyEquipmentEquipment
{
    public WeaponAttack WeaponAttack { get; }
    byte ExtraHitChance { get; }
    AmmoType AmmoType { get; }
    ShootType ShootType { get; }
    bool HasElementalDamage { get; }
    void Throw();
}
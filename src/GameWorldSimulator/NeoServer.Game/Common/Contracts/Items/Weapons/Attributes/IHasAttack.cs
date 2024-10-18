using NeoServer.Game.Item.Items.Weapons;

namespace NeoServer.Game.Common.Contracts.Items.Weapons.Attributes;

public interface IHasAttack
{
    WeaponAttack WeaponAttack { get; }
}
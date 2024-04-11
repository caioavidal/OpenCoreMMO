using NeoServer.Game.Item.Items.Weapons;

namespace NeoServer.Game.Common.Contracts.Items.Weapons;

public interface IHasAttack
{
    WeaponAttack WeaponAttack { get; }
}
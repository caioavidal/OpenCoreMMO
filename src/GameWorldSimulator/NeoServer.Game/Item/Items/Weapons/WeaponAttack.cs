using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Item.Items.Weapons;

public readonly struct WeaponAttack(IItemType metadata)
{
    public byte AttackPowerPercentage => (byte)(AttackPower * 100 / TotalAttackPower);
    public byte ElementalAttackPowerPercentage => (byte)(ElementalDamage.AttackPower * 100 / TotalAttackPower);
    public ushort TotalAttackPower => (ushort)(AttackPower + ElementalDamage.AttackPower);
    public ushort AttackPower => metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Attack);
    public (DamageType DamageType, byte AttackPower) ElementalDamage => metadata.Attributes.GetWeaponElementDamage();
}
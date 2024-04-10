using System.Buffers;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Item.Items.Weapons;

namespace NeoServer.Application.Features.Combat.Attacks.AttackSelector;

public sealed class PlayerAttackSelector
{
    public ReadOnlySpan<AttackParameter> Select(IPlayer player)
    {
        
        if (player is null)
        {
            return Span<AttackParameter>.Empty;
        }

        var elementalDamage = CalculateElementalAttack(player);
        
        var parameters = ArrayPool<AttackParameter>.Shared.Rent(1);

        parameters[0] = new()
        {
            MinDamage = player.MinimumAttackPower,
            MaxDamage = player.MaximumAttackPower,
            DamageType = DamageType.Melee,
            Name = GetAttackName(player),
            ShootType = GetShootType(player),
            ExtraAttack = elementalDamage,
            CooldownType = CooldownType.Combat
        };

        ArrayPool<AttackParameter>.Shared.Return(parameters);
        return parameters[..1].AsSpan();
    }

    private static ShootType GetShootType(IPlayer player)
    {
        var weapon = player.Inventory.Weapon;
        var ammo = player.Inventory.Ammo;

        return weapon switch
        {
            DistanceWeapon distanceWeapon when distanceWeapon.CanShootAmmunition(player.Inventory.Ammo) =>
                ammo?.ShootType ?? ShootType.None,
            IThrowableWeapon throwableDistanceWeapon => throwableDistanceWeapon.Metadata.ShootType,
            MagicWeapon magicWeapon => magicWeapon.Metadata.ShootType,
            _ => ShootType.None
        };
    }

    private string GetAttackName(IPlayer player)
    {
        var weapon = player.Inventory.Weapon;

        return weapon switch
        {
            MeleeWeapon or null => "melee",
            IDistanceWeapon or IThrowableWeapon => "distance",
            _ => null
        };
    }

    private ExtraAttack CalculateElementalAttack(ICombatActor aggressor)
    {
        if (aggressor.MaximumElementalAttackPower is 0) return default;

        var weapon = aggressor is IPlayer player ? player.Inventory.Weapon as IWeaponItem : null;
        if (weapon is null) return default;

        var damageType = weapon.WeaponAttack.ElementalDamage.DamageType;

        return new ExtraAttack
        {
            MinDamage = aggressor.MinimumAttackPower,
            MaxDamage = aggressor.MaximumElementalAttackPower,
            DamageType = damageType
        };
    }
}
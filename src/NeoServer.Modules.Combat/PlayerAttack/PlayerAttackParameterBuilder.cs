using NeoServer.BuildingBlocks.Application;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Item.Items.Weapons;
using NeoServer.Modules.Combat.Attacks.DistanceAttack;
using NeoServer.Modules.Combat.Attacks.MeleeAttack;
using NeoServer.Modules.Combat.PlayerAttack.RuneAttack;

namespace NeoServer.Modules.Combat.PlayerAttack;

public static class PlayerAttackParameterBuilder
{
    public static AttackParameter Build(IPlayer player)
    {
        if (player is null) return default;

        var elementalDamage = CalculateElementalAttack(player);

        return new AttackParameter
        {
            MinDamage = player.MinimumAttackPower,
            MaxDamage = player.MaximumAttackPower,
            DamageType = GetDamageType(player),
            Name = GetAttackName(player),
            ShootType = GetShootType(player),
            ExtraAttack = elementalDamage,
            CooldownType = CooldownType.Combat,
            IsMagicalAttack = player.Inventory.Weapon is MagicalWeapon
        };
    }

    public static AttackParameter Build(IPlayer player, IAttackRune rune, IThing target)
    {
        var area = new AreaAttackParameter();
        if (!string.IsNullOrWhiteSpace(rune.Area))
        {
            var areaEffectStore = IoC.GetInstance<IAreaEffectStore>();
            var areaTemplate = areaEffectStore.Get(rune.Area);

            area.SetArea(AreaEffect.Create(target.Location, areaTemplate), rune.Effect);
        }

        if (player is null) return default;

        var minMaxDamage = RuneAttackCalculation.Calculate(player, rune);

        return new AttackParameter
        {
            MinDamage = (ushort)minMaxDamage.Min,
            MaxDamage = (ushort)minMaxDamage.Max,
            DamageType = rune.DamageType,
            Name = "Rune",
            ShootType = rune.Metadata.ShootType,
            ExtraAttack = default,
            Cooldown = rune.CooldownTime,
            CooldownType = CooldownType.Rune,
            IsMagicalAttack = true,
            NeedTarget = rune.NeedTarget,
            Area = area,
            Effect = rune.Effect
        };
    }

    private static DamageType GetDamageType(IPlayer player)
    {
        if (player.Inventory.Weapon is null) return DamageType.Physical;

        if (player.Inventory.Weapon is MagicalWeapon) return player.Inventory.Weapon.Metadata.ShootType.ToDamageType();

        if (player.Inventory.Weapon is IDistanceWeapon)
            return player.Inventory.Ammo?.Metadata?.DamageType ?? DamageType.Physical;

        return player.Inventory.Weapon.Metadata.DamageType;
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
            MagicalWeapon magicWeapon => magicWeapon.Metadata.ShootType,
            _ => ShootType.None
        };
    }

    private static string GetAttackName(IPlayer player)
    {
        return player.Inventory.IsUsingDistanceWeapon
            ? nameof(DistanceAttackStrategy)
            : nameof(MeleeAttackStrategy);
    }

    private static ExtraAttack CalculateElementalAttack(ICombatActor aggressor)
    {
        if (aggressor.MaximumElementalAttackPower is 0) return default;

        if (aggressor is not IPlayer player) return default;

        var damageType = player.Inventory.TotalElementalAttack.DamageType;

        if (damageType == DamageType.None) return default;

        return new ExtraAttack
        {
            MinDamage = aggressor.MinimumAttackPower,
            MaxDamage = aggressor.MaximumElementalAttackPower,
            DamageType = damageType
        };
    }
}
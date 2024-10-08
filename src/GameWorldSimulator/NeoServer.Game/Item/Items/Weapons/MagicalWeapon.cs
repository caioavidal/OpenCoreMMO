using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Item.Bases;

namespace NeoServer.Game.Item.Items.Weapons;

public class MagicalWeapon(IItemType type, Location location) : Equipment(type, location), IMagicalWeapon
{
    private ShootType ShootType => Metadata.ShootType;

    private DamageType DamageType => Metadata.Attributes.HasAttribute(ItemAttribute.Damage)
        ? Metadata.DamageType
        : ShootTypeParser.ToDamageType(ShootType);

    public ushort MaxHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.MaxHitChance);
    public ushort MinHitChance => (ushort)(MaxHitChance / 2);
    public ushort ManaConsumption => Metadata.Attributes?.GetAttribute<ushort>(ItemAttribute.ManaUse) ?? 0;

    protected override string PartialInspectionText => string.Empty;
    public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);
    public WeaponType WeaponType => WeaponType.Magical;

    public bool CanShootAmmunition(IAmmo ammo) => false;

    public bool Attack(ICombatActor actor, ICombatActor enemy, out CombatAttackResult combatResult)
    {
        // combatResult = new CombatAttackResult(ShootType);
        //
        // if (actor is not IPlayer player) return false;
        // if (!player.HasEnoughMana(ManaConsumption)) return false;
        //
        // var combat = new CombatAttackValue((ushort)(MaxDamage / 2), MaxDamage, Range, DamageType);
        //
        // if (DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
        // {
        //     player.ConsumeMana(ManaConsumption);
        //     //   enemy.ReceiveAttackFrom(actor, damage);
        //     return true;
        // }
        //
        // return false;
        throw new NotImplementedException();
    }

    public override bool CanBeDressed(IPlayer player)
    {
        if (Guard.IsNullOrEmpty(Vocations)) return true;

        foreach (var vocation in Vocations)
            if (vocation == player.VocationType)
                return true;

        return false;
    }

    public void OnMoved(IThing to)
    {
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.MagicWeapon;
    }
}
using System.Text;
using NeoServer.Game.Combat.Attacks.Legacy;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Contracts.Items.Weapons.Attributes;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Item.Bases;

namespace NeoServer.Game.Item.Items.Weapons;

public class DistanceWeapon(IItemType type, Location location)
    : Equipment(type, location), IDistanceWeapon, IHasAttackBonus, INeedsAmmo
{
    protected override string PartialInspectionText
    {
        get
        {
            var range = Range > 0 ? $"Range: {Range}" : string.Empty;
            var atk = AttackBonus > 0 ? $"Atk: {AttackBonus:+#}" : string.Empty;
            var hit = ExtraHitChance != 0 ? $"Hit% {ExtraHitChance:+#;-#}" : string.Empty;

            if (Guard.AllNullOrEmpty(range, atk, hit)) return string.Empty;

            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(range)) stringBuilder.Append($"{range}, ");
            if (!string.IsNullOrWhiteSpace(atk)) stringBuilder.Append($"{atk}, ");
            if (!string.IsNullOrWhiteSpace(hit)) stringBuilder.Append($"{hit}, ");

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            return stringBuilder.ToString();
        }
    }

    public override bool CanBeDressed(IPlayer player)
    {
        if (Guard.IsNullOrEmpty(Vocations)) return true;

        foreach (var vocation in Vocations)
            if (vocation == player.VocationType)
                return true;

        return false;
    }

    public bool CanShootAmmunition(IAmmo ammo) => Metadata.AmmoType == (ammo?.AmmoType ?? AmmoType.None);

    public byte AttackBonus => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);
    public sbyte ExtraHitChance => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.HitChance);
    public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);
    public void OnMoved(IThing to)
    {
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.DistanceWeapon;
    }

    private void UseElementalDamage(ICombatActor actor, ICombatActor enemy, ref CombatAttackResult combatResult,
        ref bool result, IPlayer player, IAmmo ammo, ref ushort maxDamage, ref CombatAttackValue combat)
    {
        if (!ammo.HasElementalDamage) return;

        maxDamage = 100; //player.CalculateAttackPower(0.09f, (ushort)(ammo.ElementalDamage.Item2 + ExtraAttack)); //TODO
        combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range,
            ammo.WeaponAttack.ElementalDamage.DamageType);

        if (!DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var elementalDamage)) return;

        combatResult.DamageType = ammo.WeaponAttack.ElementalDamage.DamageType;

        //enemy.ReceiveAttackFrom(actor, elementalDamage);
        result = true;
    }

    public bool Attack(ICombatActor actor, ICombatActor enemy, out CombatAttackResult combat)
    {
        throw new NotImplementedException();
    }
}
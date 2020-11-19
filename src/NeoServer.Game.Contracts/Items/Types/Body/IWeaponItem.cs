using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Players;
using System;

namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate bool OnAttack(ICombatActor actor, ICombatActor enemy, DamageType damageType, int minDamage, int maxDamage, out CombatDamage damage);

    public interface IWeapon : IBodyEquipmentItem
    {
        bool TwoHanded => Metadata.BodyPosition == Slot.TwoHanded;

        Slot Slot => Slot.Left;
        public WeaponType Type => Metadata.WeaponType;

        bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combat);
    }
    public interface IWeaponItem : IWeapon, IBodyEquipmentItem
    {
        ushort Attack { get; }
        byte Defense { get; }

        Tuple<DamageType, byte> ElementalDamage { get; }
        sbyte ExtraDefense { get; }

    }
}

using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class WeaponItem : MoveableItem, IWeaponItem
    {
        public WeaponItem(IItemType itemType, Location location) : base(itemType, location)
        {
            //AllowedVocations  todo
        }

        public ushort Attack => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Attack);

        public byte Defense => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Defense);

        public Tuple<DamageType, byte> ElementalDamage => Metadata.Attributes.GetWeaponElementDamage();

        public sbyte ExtraDefense => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);

        public ImmutableHashSet<VocationType> AllowedVocations { get; }



        public static bool IsApplicable(IItemType type) =>
            type.WeaponType switch
            {
                WeaponType.Axe => true,
                WeaponType.Club => true,
                WeaponType.Sword => true,
                _ => false
            };

    }
}

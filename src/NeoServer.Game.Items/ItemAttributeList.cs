using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Items.Parsers;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoServer.Game.Items
{


    public sealed class ItemAttributeList : IItemAttributeList
    {
        private IDictionary<ItemAttribute, IConvertible> _defaultAttributes;

        public ItemAttributeList()
        {
            _defaultAttributes = new Dictionary<ItemAttribute, IConvertible>();
        }

        public void SetAttribute(ItemAttribute attribute, int attributeValue) => _defaultAttributes[attribute] = attributeValue;

        public void SetAttribute(ItemAttribute attribute, IConvertible attributeValue) => _defaultAttributes[attribute] = attributeValue;

        public void SetAttribute(string attributeName, int attributeValue)
        {
            if (!Enum.TryParse(attributeName, out ItemAttribute attribute))
            {
                throw new InvalidDataException($"Attempted to set an unknown Item attribute [{attributeName}].");
            }

            _defaultAttributes[attribute] = attributeValue;
        }

        public bool HasAttribute(ItemAttribute attribute) => _defaultAttributes.ContainsKey(attribute);

        public T GetAttribute<T>(ItemAttribute attribute) where T : struct
        {
            IConvertible value = null;

            if (_defaultAttributes?.TryGetValue(attribute, out value) ?? false)
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default;
        }
        public string GetAttribute(ItemAttribute attribute)
        {
            IConvertible value = null;

            if (_defaultAttributes?.TryGetValue(attribute, out value) ?? false)
            {
                return (string)value;
            }


            return default;
        }


        public FloorChangeDirection GetFloorChangeDirection()
        {
            if (_defaultAttributes?.ContainsKey(ItemAttribute.FloorChange) ?? false)
            {
                var floorChange = GetAttribute(ItemAttribute.FloorChange);

                return floorChange switch
                {
                    "down" => FloorChangeDirection.Down,
                    "north" => FloorChangeDirection.North,
                    "south" => FloorChangeDirection.South,
                    "southalt" => FloorChangeDirection.SouthAlternative,
                    "west" => FloorChangeDirection.West,
                    "east" => FloorChangeDirection.East,
                    "eastalt" => FloorChangeDirection.EastAlternative,
                    _ => FloorChangeDirection.None
                };
            }

            return FloorChangeDirection.None;
        }

        public ushort GetTransformationItem()
        {
            if (_defaultAttributes?.ContainsKey(ItemAttribute.TransformEquipTo) ?? false)
                return GetAttribute<ushort>(ItemAttribute.TransformEquipTo);

            if (_defaultAttributes?.ContainsKey(ItemAttribute.TransformDequipTo) ?? false)
                return GetAttribute<ushort>(ItemAttribute.TransformDequipTo);

            return 0;
        }

        public Tuple<DamageType, byte> GetWeaponElementDamage()
        {
            if (_defaultAttributes?.ContainsKey(ItemAttribute.ElementEarth) ?? false)
                return new Tuple<DamageType, byte>(DamageType.Earth, GetAttribute<byte>(ItemAttribute.ElementEarth));

            if (_defaultAttributes?.ContainsKey(ItemAttribute.ElementEnergy) ?? false)
                return new Tuple<DamageType, byte>(DamageType.Energy, GetAttribute<byte>(ItemAttribute.ElementEnergy));

            if (_defaultAttributes?.ContainsKey(ItemAttribute.ElementFire) ?? false)
                return new Tuple<DamageType, byte>(DamageType.Fire, GetAttribute<byte>(ItemAttribute.ElementFire));

            if (_defaultAttributes?.ContainsKey(ItemAttribute.ElementIce) ?? false)
                return new Tuple<DamageType, byte>(DamageType.Ice, GetAttribute<byte>(ItemAttribute.ElementIce));

            return null;
        }
        public Dictionary<SkillType, byte> SkillBonus
        {
            get
            {
                var dictionary = new Dictionary<SkillType, byte>();

                if (_defaultAttributes?.ContainsKey(ItemAttribute.SkillAxe) ?? false)
                    dictionary.TryAdd(SkillType.Axe, GetAttribute<byte>(ItemAttribute.SkillAxe));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.SkillClub) ?? false)
                    dictionary.TryAdd(SkillType.Club, GetAttribute<byte>(ItemAttribute.SkillClub));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.SkillDistance) ?? false)
                    dictionary.TryAdd(SkillType.Distance, GetAttribute<byte>(ItemAttribute.SkillDistance));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.SkillFishing) ?? false)
                    dictionary.TryAdd(SkillType.Fishing, GetAttribute<byte>(ItemAttribute.SkillFishing));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.SkillFist) ?? false)
                    dictionary.TryAdd(SkillType.Fist, GetAttribute<byte>(ItemAttribute.SkillFist));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.SkillShield) ?? false)
                    dictionary.TryAdd(SkillType.Shielding, GetAttribute<byte>(ItemAttribute.SkillShield));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.SkillSword) ?? false)
                    dictionary.TryAdd(SkillType.Sword, GetAttribute<byte>(ItemAttribute.SkillSword));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.Speed) ?? false)
                    dictionary.TryAdd(SkillType.Speed, GetAttribute<byte>(ItemAttribute.Speed));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.MagicPoints) ?? false)
                    dictionary.TryAdd(SkillType.Magic, GetAttribute<byte>(ItemAttribute.MagicPoints));

                return dictionary;
            }
        }

        public Dictionary<DamageType, byte> DamageProtection
        {
            get
            {
                var dictionary = new Dictionary<DamageType, byte>();

                if (_defaultAttributes?.ContainsKey(ItemAttribute.AbsorbPercentDeath) ?? false)
                    dictionary.TryAdd(DamageType.Death, GetAttribute<byte>(ItemAttribute.AbsorbPercentDeath));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.AbsorbPercentPhysical) ?? false)
                    dictionary.TryAdd(DamageType.AbsorbPercentPhysical, GetAttribute<byte>(ItemAttribute.AbsorbPercentPhysical));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.AbsorbPercentMagic) ?? false)
                    dictionary.TryAdd(DamageType.AbsorbPercentMagic, GetAttribute<byte>(ItemAttribute.AbsorbPercentMagic));

                if (_defaultAttributes?.ContainsKey(ItemAttribute.AbsorbPercentAll) ?? false)
                    dictionary.TryAdd(DamageType.Death, GetAttribute<byte>(ItemAttribute.AbsorbPercentAll));

                return dictionary;
            }
        }


    }
}

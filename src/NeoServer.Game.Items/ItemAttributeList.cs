using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using System;
using System.Collections.Generic;
using System.IO;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Common.Parsers;

namespace NeoServer.Game.Items
{

    public sealed class ItemAttributeList : IItemAttributeList
    {
        private IDictionary<ItemAttribute, (dynamic, IItemAttributeList)> _defaultAttributes;

        public ItemAttributeList()
        {
            _defaultAttributes = new Dictionary<ItemAttribute, (dynamic, IItemAttributeList)>();
        }

        public void SetAttribute(ItemAttribute attribute, int attributeValue) => _defaultAttributes[attribute] = (attributeValue, null);

        public void SetAttribute(ItemAttribute attribute, IConvertible attributeValue) => _defaultAttributes[attribute] = (attributeValue, null);
        public void SetAttribute(ItemAttribute attribute, dynamic values) => _defaultAttributes[attribute] = (values, null);

        public void SetAttribute(ItemAttribute attribute, IConvertible attributeValue, IItemAttributeList attrs) => _defaultAttributes[attribute] = (attributeValue, attrs);

        public bool HasAttribute(ItemAttribute attribute) => _defaultAttributes.ContainsKey(attribute);

        public T GetAttribute<T>(ItemAttribute attribute) where T : struct
        {
            if (_defaultAttributes is null) return default;

            if (_defaultAttributes.TryGetValue(attribute, out var value))
            {
                return (T)Convert.ChangeType(value.Item1, typeof(T));
            }

            return default;
        }
        public string GetAttribute(ItemAttribute attribute)
        {
            if (_defaultAttributes is null) return default;

            if (_defaultAttributes.TryGetValue(attribute, out var value))
            {
                return (string)value.Item1;
            }

            return default;
        }
        public dynamic[] GetAttributeArray(ItemAttribute attribute)
        {
            if (_defaultAttributes is null) return default;

            if (_defaultAttributes.TryGetValue(attribute, out var value))
            {
                return value.Item1;
            }

            return default;
        }

        public IItemAttributeList GetInnerAttributes(ItemAttribute attribute)
        {
            if (_defaultAttributes is null) return default;

            if (_defaultAttributes.TryGetValue(attribute, out var value))
            {
                return value.Item2;
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
                    "up" => FloorChangeDirection.Up,
                    _ => FloorChangeDirection.None
                };
            }

            return FloorChangeDirection.None;
        }

        public VocationType[] GetRequiredVocations()
        {
            if (_defaultAttributes is null) return default;

            if (_defaultAttributes.TryGetValue(ItemAttribute.Vocation, out var value))
            {
                return (VocationType[])value.Item1;
            }

            return default;
        }
        public EffectT GetEffect()
        {
            if (_defaultAttributes?.ContainsKey(ItemAttribute.Effect) ?? false)
            {
                var effect = GetAttribute(ItemAttribute.Effect);

                return effect switch
                {
                    "teleport" => EffectT.BubbleBlue,
                    "blueshimmer" => EffectT.GlitterBlue,
                    "bluebubble" => EffectT.BubbleBlue,
                    "greenbubble" => EffectT.GlitterGreen,
                    _ => EffectT.None
                };
            }

            return EffectT.None;
        }

        public ushort GetTransformationItem()
        {
            if (_defaultAttributes?.ContainsKey(ItemAttribute.TransformEquipTo) ?? false)
                return GetAttribute<ushort>(ItemAttribute.TransformEquipTo);

            if (_defaultAttributes?.ContainsKey(ItemAttribute.TransformDequipTo) ?? false)
                return GetAttribute<ushort>(ItemAttribute.TransformDequipTo);

            if (_defaultAttributes?.ContainsKey(ItemAttribute.TransformTo) ?? false)
                return GetAttribute<ushort>(ItemAttribute.TransformTo);

            return 0;
        }

        public Tuple<DamageType, byte> GetWeaponElementDamage()
        {
            if (_defaultAttributes?.ContainsKey(ItemAttribute.ElementEarth) ?? false)
                return new Tuple<DamageType, byte>(DamageType.Earth, GetAttribute<byte>(ItemAttribute.ElementEarth));

            if (_defaultAttributes?.ContainsKey(ItemAttribute.ElementEnergy) ?? false)
                return new Tuple<DamageType, byte>(DamageType.Energy, GetAttribute<byte>(ItemAttribute.ElementEnergy));

            if (_defaultAttributes?.ContainsKey(ItemAttribute.ElementFire) ?? false)
                return new Tuple<DamageType, byte>(DamageType.FireField, GetAttribute<byte>(ItemAttribute.ElementFire)); //todo

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

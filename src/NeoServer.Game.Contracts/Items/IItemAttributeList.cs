using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemAttributeList
    {
        Dictionary<DamageType, byte> DamageProtection { get; }
        Dictionary<SkillType, byte> SkillBonus { get; }

        string GetAttribute(ItemAttribute attribute);
        T GetAttribute<T>(ItemAttribute attribute) where T : struct;
        FloorChangeDirection GetFloorChangeDirection();
        Tuple<DamageType, byte> GetWeaponElementDamage();
        bool HasAttribute(ItemAttribute attribute);
        void SetAttribute(ItemAttribute attribute, IConvertible attributeValue);
        void SetAttribute(ItemAttribute attribute, int attributeValue);
        void SetAttribute(string attributeName, int attributeValue);
    }
}

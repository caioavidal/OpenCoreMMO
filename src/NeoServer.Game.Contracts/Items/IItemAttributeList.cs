using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
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
        IItemAttributeList GetInnerAttributes(ItemAttribute attribute);
        ushort GetTransformationItem();
        Tuple<DamageType, byte> GetWeaponElementDamage();
        bool HasAttribute(ItemAttribute attribute);
        void SetAttribute(ItemAttribute attribute, IConvertible attributeValue);
        void SetAttribute(ItemAttribute attribute, int attributeValue);
        void SetAttribute(ItemAttribute attribute, IConvertible attributeValue, IItemAttributeList attrs);
    }
}

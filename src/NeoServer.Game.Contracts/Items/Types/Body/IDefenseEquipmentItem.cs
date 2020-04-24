using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IDefenseEquipmentItem: IBodyEquipmentItem
    {
        ImmutableDictionary<DamageType, byte> DamageProtection { get; }
        ushort DefenseValue { get; }
        ImmutableDictionary<SkillType, byte> SkillBonus { get; }
    }
}

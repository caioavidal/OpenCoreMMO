using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using System.Collections.Immutable;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IDefenseEquipmentItem : IBodyEquipmentItem
    {
        ImmutableDictionary<DamageType, byte> DamageProtection { get; }
        ushort DefenseValue => Metadata.Attributes.HasAttribute(ItemAttribute.WeaponDefendValue) ?
            Metadata.Attributes.GetAttribute<byte>(ItemAttribute.WeaponDefendValue) : Metadata.Attributes.GetAttribute<byte>(ItemAttribute.ArmorValue);

        ushort ArmorValue => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.ArmorValue);

        private string DefenseText => ArmorValue > 0 ? $"(Arm:{ArmorValue})" : $"(Def:{DefenseValue})";
        
        string IThing.InspectionText => $"{LookText} {DefenseText}{RequirementText}";
    }
}

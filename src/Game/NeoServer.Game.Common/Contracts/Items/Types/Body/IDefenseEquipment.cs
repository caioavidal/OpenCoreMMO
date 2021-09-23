using System.Collections.Immutable;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface IDefenseEquipment : IBodyEquipmentItem, IEquipment
    {

        ushort DefenseValue => Metadata.Attributes.HasAttribute(ItemAttribute.WeaponDefendValue)
            ? Metadata.Attributes.GetAttribute<byte>(ItemAttribute.WeaponDefendValue)
            : Metadata.Attributes.GetAttribute<byte>(ItemAttribute.ArmorValue);

        ushort ArmorValue => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.ArmorValue);

        private string DefenseText => ArmorValue > 0 ? $"(Arm:{ArmorValue})" : $"(Def:{DefenseValue})";

        string IThing.InspectionText => $"{LookText} {DefenseText}{RequirementText}";
    }
}
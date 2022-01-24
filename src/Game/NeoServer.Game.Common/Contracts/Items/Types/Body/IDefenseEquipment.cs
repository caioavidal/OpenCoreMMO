using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body;

public interface IDefenseEquipment : IBodyEquipmentEquipment, IEquipment
{
    ushort DefenseValue => Metadata.Attributes.HasAttribute(ItemAttribute.WeaponDefendValue)
        ? Metadata.Attributes.GetAttribute<byte>(ItemAttribute.WeaponDefendValue)
        : Metadata.Attributes.GetAttribute<byte>(ItemAttribute.ArmorValue);

    ushort ArmorValue => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.ArmorValue);
}
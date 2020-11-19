namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IDistanceWeaponItem : IWeapon, IBodyEquipmentItem
    {
        byte ExtraAttack => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Attack);
        byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.HitChance);
        byte Range => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);
    }

    
}
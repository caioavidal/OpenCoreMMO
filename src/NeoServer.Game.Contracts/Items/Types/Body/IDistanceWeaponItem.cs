namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IDistanceWeaponItem : IWeapon, IBodyEquipmentItem
    {
        byte ExtraAttack => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Attack);
        byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.HitChance);
        byte Range => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Range);
    }

    
}
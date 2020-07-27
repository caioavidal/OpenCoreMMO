namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IDistanceWeaponItem : IWeapon,IBodyEquipmentItem
    {
        byte MaxAttackDistance { get; }
        byte Attack => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Attack);
        byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.HitChance);
        //bool TwoHanded { get; }
        byte Range => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);
    }
}
namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IAmmoItem : IBodyEquipmentItem
    {
        byte Range { get; }
        byte Attack { get; }
        byte ExtraHitChance { get; }
    }
}
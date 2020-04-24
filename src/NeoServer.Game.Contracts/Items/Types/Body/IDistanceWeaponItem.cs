namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IDistanceWeaponItem : IBodyEquipmentItem
    {
        byte MaxAttackDistance { get; }
        byte Attack { get; }
        byte ExtraHitChance { get; }
        bool TwoHanded { get; }
    }
}
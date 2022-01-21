namespace NeoServer.Game.Common.Contracts.Items
{
    public delegate void TransformEquipment(IItemType before, IItemType now);
    public interface ITransformableEquipment
    {
        void TransformOnEquip();
        void TransformOnDequip();
        IItemType TransformEquipItem { get; }
        IItemType TransformDequipItem { get; }
        event TransformEquipment OnTransformed;
    }
}

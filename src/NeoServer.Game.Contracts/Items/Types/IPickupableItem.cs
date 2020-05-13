namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IPickupableItem : IMoveableThing, IItem
    {
        float Weight => Metadata.Attributes.GetAttribute<float>(Enums.ItemAttribute.Weight);
    }
}

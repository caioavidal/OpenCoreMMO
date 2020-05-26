namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IPickupable : IMoveableThing, IItem
    {
        float Weight => Metadata.Attributes.GetAttribute<float>(Enums.ItemAttribute.Weight);
    }
}

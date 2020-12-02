namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IPickupable : IMoveableThing, IItem
    {
        float Weight => Metadata.Attributes.GetAttribute<float>(Common.ItemAttribute.Weight);
        string IThing.CloseInspectionText => $"{InspectionText}.\nIt weighs {Weight} oz{DescriptionText}";
        string DescriptionText => Metadata.Description is null ? string.Empty : $"\n{Metadata.Description}";
    }
}

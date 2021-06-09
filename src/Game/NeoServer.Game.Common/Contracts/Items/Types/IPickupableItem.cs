namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IPickupable : IMoveableThing, IItem
    {
        float Weight => Metadata.Weight;
        string DescriptionText => Metadata.Description is null ? string.Empty : $"\n{Metadata.Description}";
        string IThing.CloseInspectionText => $"{InspectionText}.\nIt weighs {Weight} oz{DescriptionText}";
    }
}
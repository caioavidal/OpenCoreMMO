using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface IThing
    {
        Location Location { get; set; }
        string Name { get; }
        string InspectionText => $"{Name}";
        string CloseInspectionText => InspectionText;
        public byte Amount => 1;
       // IStore StoredIn { get; protected set; }
      //  public void SetStoredPlace(IStore store) => StoredIn = store;
    }
}
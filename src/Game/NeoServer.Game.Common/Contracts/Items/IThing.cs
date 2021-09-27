namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IThing
    {
        Location.Structs.Location Location { get; set; }
        string Name { get; }

        public byte Amount => 1;

        string GetLookText(bool isClose = false);
        // IStore StoredIn { get; protected set; }
        //  public void SetStoredPlace(IStore store) => StoredIn = store;
    }
}
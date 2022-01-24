namespace NeoServer.Game.Common.Contracts.Items;

public interface IMoveableThing : IThing
{
    void OnMoved();

    void SetNewLocation(Location.Structs.Location location)
    {
        Location = location;
    }
}
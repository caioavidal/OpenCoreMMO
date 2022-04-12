namespace NeoServer.Game.Common.Contracts.Items;

public interface IMovableThing : IThing
{
    void OnMoved(IThing to);
    void SetNewLocation(Location.Structs.Location location) => Location = location;
}
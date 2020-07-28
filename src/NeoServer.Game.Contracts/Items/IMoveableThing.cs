using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface IMoveableThing : IThing
    {
        IMoveableThing Clone();
        void SetNewLocation(Location location);
    }
}

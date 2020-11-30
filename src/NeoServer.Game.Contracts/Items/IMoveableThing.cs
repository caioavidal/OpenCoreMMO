using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface IMoveableThing : IThing
    {

        void SetNewLocation(Location location)
        {
            Location = location;
        }
    }
}

using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Model
{
    public abstract class MoveableThing : IMoveableThing
    {

        private Location location;
        public Location Location => location;

        public string Name { get; protected set; }

        public abstract string InspectionText { get; }

        public abstract string CloseInspectionText { get; }

        public abstract void Moved(ITile fromTile, ITile toTile);

        public void SetNewLocation(Location location)
        {
            this.location = location;
        }
    }
}

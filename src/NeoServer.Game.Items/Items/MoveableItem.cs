using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public abstract class MoveableItem : BaseItem, IMoveableThing
    {
        public MoveableItem(IItemType type) : base(type)
        {
        }

        private Location location;
        public override Location Location => location;

        public void SetNewLocation(Location location)
        {
            this.location = location;
        }
    }
}

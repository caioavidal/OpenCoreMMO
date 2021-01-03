using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Items.Items
{
    public abstract class MoveableItem : BaseItem, IMoveableThing
    {
        public MoveableItem(IItemType type, Location location) : base(type)
        {
            Location = location;
        }
        public float Weight => Metadata.Weight;

        public IMoveableThing Clone()
        {
            var clone = (IMoveableThing)MemberwiseClone();
            return clone;
        }

        public virtual void OnMoved() { }
    }
}

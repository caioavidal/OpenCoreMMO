using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public abstract class MoveableItem : BaseItem, IMoveableThing
    {
        public MoveableItem(IItemType type, Location location) : base(type)
        {
            Location = location;
        }

        public float Weight => Metadata.Weight;

        public virtual void OnMoved()
        {
        }

        public IMoveableThing Clone()
        {
            var clone = (IMoveableThing) MemberwiseClone();
            return clone;
        }
    }
}
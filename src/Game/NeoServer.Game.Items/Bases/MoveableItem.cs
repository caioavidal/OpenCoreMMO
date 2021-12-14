using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items;

namespace NeoServer.Game.Items.Bases
{
    public abstract class MoveableItem : BaseItem, IMoveableThing
    {
        protected MoveableItem(IItemType type, Location location) : base(type,location)
        {
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
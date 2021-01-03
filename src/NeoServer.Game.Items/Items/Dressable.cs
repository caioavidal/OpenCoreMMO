using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Game.Items.Items
{
    public class Dressable : MoveableItem, IPickupable
    {
        public Dressable(IItemType type, Location location) : base(type, location)
        {
        }

        public bool InUse { get; private set; }

        public void Dress() => InUse = true;
        public void Undress() => InUse = false;

    }
}

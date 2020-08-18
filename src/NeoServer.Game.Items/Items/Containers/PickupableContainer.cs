using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public class PickupableContainer : Container, IPickupableContainer
    {
        public PickupableContainer(IItemType type, Location location) : base(type, location)
        {
            Weight = Metadata.Weight;

            OnItemAdded += IncreaseWeight;
            OnItemRemoved += (slot, item) => DecreaseWeight(item);
            OnItemUpdated += (slot, item, amount) => UpdateWeight(amount);
        }

        private void UpdateWeight(sbyte amount) => Weight += amount;

        private void IncreaseWeight(IItem item)
        {
            Weight += item is IPickupable pickupableItem ? pickupableItem.Weight : 0;
        }
        private void DecreaseWeight(IItem item) => Weight -= item is IPickupable pickupableItem ? pickupableItem.Weight : 0;

        public new float Weight { get; private set; }

        public static new bool IsApplicable(IItemType type) => (type.Group == Enums.ItemGroup.GroundContainer ||
            type.Attributes.GetAttribute(Enums.ItemAttribute.Type)?.ToLower() == "container") && type.HasFlag(Enums.ItemFlag.Pickupable);
    }
}

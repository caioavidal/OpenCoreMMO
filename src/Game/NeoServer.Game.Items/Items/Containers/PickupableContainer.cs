using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers
{
    public class PickupableContainer : Container, IPickupableContainer
    {
        public PickupableContainer(IItemType type, Location location) : base(type, location)
        {
            Weight = Metadata.Weight;

            OnItemAdded += IncreaseWeight;
            OnItemRemoved += DecreaseWeight;
            OnItemUpdated += UpdateWeight;
        }

        public new float Weight { get; set; }

        private void UpdateWeight(byte slot, IItem tem, sbyte amount)
        {
            Weight += amount;
            UpdateParents(amount);
        }

        private void IncreaseWeight(IItem item)
        {
            var weight = item is IPickupable pickupableItem ? pickupableItem.Weight : 0;
            Weight += weight;
            UpdateParents(weight);
        }

        private void UpdateParents(float weight)
        {
            var parent = Parent;
            while (parent is IPickupableContainer container)
            {
                container.Weight += weight;
                parent = container.Parent;
            }
        }

        private void DecreaseWeight(byte slot, IItem item)
        {
            var weight = item is IPickupable pickupableItem ? pickupableItem.Weight : 0;
            Weight -= weight;
            UpdateParents(-weight);
        }

        public new static bool IsApplicable(IItemType type)
        {
            return (type.Group == ItemGroup.GroundContainer ||
                    type.Attributes.GetAttribute(ItemAttribute.Type)?.ToLower() == "container") &&
                   type.HasFlag(ItemFlag.Pickupable);
        }
    }
}
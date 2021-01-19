using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Items.Items
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
            IThing parent = Parent;
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

        public new float Weight { get; set; }

        public static new bool IsApplicable(IItemType type) => (type.Group == Common.ItemGroup.GroundContainer ||
             type.Attributes.GetAttribute(Common.ItemAttribute.Type)?.ToLower() == "container") && type.HasFlag(Common.ItemFlag.Pickupable);
    }
}

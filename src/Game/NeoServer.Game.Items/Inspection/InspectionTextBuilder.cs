using System.Globalization;
using System.Text;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Inspection
{
    public class InspectionTextBuilder:IInspectionTextBuilder
    {
        private readonly IVocationStore _vocationStore;
        public InspectionTextBuilder(IVocationStore vocationStore)
        {
            _vocationStore = vocationStore;
        }
        public string Build(IThing thing, bool isClose = false)
        {
            if (thing is not IItem item) return string.Empty;
            
            var inspectionText = new StringBuilder();

            AddItemName(item, inspectionText);
            AddEquipmentAttributes(item, inspectionText);
            inspectionText.AppendLine();
            AddRequirement(item, inspectionText);
            
            AddWeight(item, isClose, inspectionText);
            AddDescription(item, inspectionText);
            return inspectionText.ToString();
        }
        public bool IsApplicable(IThing thing) => thing is IItem;

     
        private void AddRequirement(IItem item, StringBuilder inspectionText)
        {
            var result = RequirementInspectionTextBuilder.Build(item, _vocationStore);
            if (string.IsNullOrWhiteSpace(result)) return;
            inspectionText.AppendLine(result);
        }
        private static void AddDescription(IItem item, StringBuilder inspectionText)
        {
            if (!string.IsNullOrWhiteSpace(item.Metadata.Description))
            {
                inspectionText.AppendLine(item.Metadata.Description);
            }
        }

        private static void AddWeight(IItem item, bool isClose, StringBuilder inspectionText)
        {
            if (item is IPickupable pickupable && isClose)
            {
                inspectionText.AppendLine(
                    $"{(item is ICumulative ? "They weigh" : "It weighs")} {pickupable.Weight.ToString("F", CultureInfo.InvariantCulture)} oz.");
            }
        }

        private static void AddItemName(IItem item, StringBuilder inspectionText)
        {
            inspectionText.Append("You see ");

            inspectionText.Append(item is ICumulative cumulative
                ? $"{cumulative.Amount} {item.Name}{(cumulative.Amount > 1 ? "s" : "")}"
                : $"{item.Metadata.Article} {item.Name}");
        }

        private static void AddEquipmentAttributes(IItem item, StringBuilder inspectionText)
        {
            if (item is IEquipment && !string.IsNullOrWhiteSpace(item.InspectionText))
            {
                inspectionText.Append($" {item.InspectionText}");
            }
        }
    }
}

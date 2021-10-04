using System.Globalization;
using System.Text;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Items.Inspection
{
    public static class InspectionTextBuilder
    {
        public static string Build(this IItem item, bool isClose = false)
        {
            var inspectionText = new StringBuilder();

            AddItemName(item, inspectionText);
            AddEquipmentAttributes(item, inspectionText);
            inspectionText.AppendLine();
            AddRequirement(item, inspectionText);
            
            AddWeight(item, isClose, inspectionText);
            AddDescription(item, inspectionText);

            return inspectionText.ToString();
        }

        private static void AddRequirement(IItem item, StringBuilder inspectionText)
        {
            var result = RequirementInspectionTextBuilder.Build(item);
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

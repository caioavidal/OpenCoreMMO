using System.Globalization;//i
using System.Text;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Items
{
    public static class InspectionTextBuilder
    {
        public static string BuildLookText(this IItem item, bool isClose = false)
        {
            var inspectionText = new StringBuilder();

            AddItemName(item, inspectionText);
            AddEquipmentAttributes(item, inspectionText);
            inspectionText.AppendLine();
            
            AddItemRequirementText(item,inspectionText);
            AddWeight(item, isClose, inspectionText);
            AddDescription(item, inspectionText);

            return inspectionText.ToString();
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

        private static void AddItemRequirementText(IItem item, StringBuilder inspectionText)
        {
            if (item is not IItemRequirement itemRequirement) return;

            var vocations = itemRequirement.Vocations;
            var minLevel = itemRequirement.MinLevel;
            
            if (Guard.IsNullOrEmpty(vocations) && minLevel == 0) return;

            string FormatVocations(byte[] allVocations)
            {
                if (Guard.IsNullOrEmpty(allVocations)) return "Players";
                var text = new StringBuilder();
                for (var i = 0; i < allVocations.Length; i++)
                {
                    if (!VocationStore.Data.TryGetValue(allVocations[i], out var vocation)) continue;
                    text.Append($"{vocation.Name}s");

                    var lastItem = i == allVocations.Length - 1;
                    var penultimate = i == allVocations.Length - 2;

                    if (lastItem) continue;
                    if (penultimate)
                    {
                        text.Append(" and ");
                        continue;
                    }

                    text.Append(", ");
                }

                var finalText = text.ToString();
                return string.IsNullOrWhiteSpace(finalText) ? "Players" : text.ToString();
            }

            var vocationsText = FormatVocations(vocations);

            inspectionText.AppendLine($"It can only be wielded properly by {vocationsText}{(minLevel > 0 ? $" of level {minLevel} or higher" : string.Empty)}.");
        }
    }
}

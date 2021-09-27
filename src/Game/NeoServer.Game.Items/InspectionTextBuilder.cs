using System.Globalization;
using System.Text;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;

namespace NeoServer.Game.Items
{
    public static class InspectionTextBuilder
    {
        public static string BuildLookText(this IItem item, bool isClose = false)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("You see ");

            stringBuilder.Append(item is ICumulative cumulative
                ? $"{cumulative.Amount} {item.Name}{(cumulative.Amount > 1 ? "s" : "")}"
                : $"{item.Metadata.Article} {item.Name}");

            if (item is IEquipment && !string.IsNullOrWhiteSpace(item.InspectionText))
            {
                stringBuilder.Append($" {item.InspectionText}");
            }

            stringBuilder.AppendLine();

            if (item is IPickupable pickupable && isClose)
            {
                stringBuilder.AppendLine($"{(item is ICumulative ? "They weigh" : "It weighs")} {pickupable.Weight.ToString("F", CultureInfo.InvariantCulture)} oz");
            }

            if (!string.IsNullOrWhiteSpace(item.Metadata.Description))
            {
                stringBuilder.AppendLine(item.Metadata.Description);
            }

            return stringBuilder.ToString();
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Items.Inspection;

public class InspectionTextBuilder : IInspectionTextBuilder
{
    private readonly IVocationStore _vocationStore;

    public InspectionTextBuilder(IVocationStore vocationStore)
    {
        _vocationStore = vocationStore;
    }

    public string Build(IThing thing, IPlayer player, bool isClose = false)
    {
        if (thing is not IItem item) return string.Empty;

        var inspectionText = new StringBuilder();

        AddItemName(item, player, inspectionText);
        AddEquipmentAttributes(item, inspectionText);
        inspectionText.AppendLine();
        AddRequirement(item, inspectionText);

        AddWeight(item, isClose, inspectionText);
        AddDescription(item, inspectionText);

        var finalText = inspectionText.ToString().TrimNewLine().AddEndOfSentencePeriod;

        return $"{finalText}";
    }

    public bool IsApplicable(IThing thing)
    {
        return thing is IItem;
    }

    private void AddRequirement(IItem item, StringBuilder inspectionText)
    {
        var result = RequirementInspectionTextBuilder.Build(item, _vocationStore);
        if (string.IsNullOrWhiteSpace(result)) return;
        inspectionText.AppendLine(result);
    }

    private static void AddDescription(IItem item, StringBuilder inspectionText)
    {
        if (!string.IsNullOrWhiteSpace(item.Metadata.Description)) inspectionText.AppendLine(item.Metadata.Description);
    }

    private static void AddWeight(IItem item, bool isClose, StringBuilder inspectionText)
    {
        if (item is IPickupable pickupable && isClose)
            inspectionText.AppendLine(
                $"{(item is ICumulative ? "They weigh" : "It weighs")} {pickupable.Weight.ToString("F", CultureInfo.InvariantCulture)} oz.");
    }

    private static void AddItemName(IItem item, IPlayer player, StringBuilder inspectionText)
    {
        if (player.CanSeeInspectionDetails)
            inspectionText.AppendLine($"Id: [{item.ServerId}] - Pos: {item.Location}");

        inspectionText.Append("You see ");
        inspectionText.Append(item is ICumulative cumulative
            ? $"{cumulative.Amount} {item.Name}{(cumulative.Amount > 1 ? "s" : "")}"
            : $"{item.Metadata.Article} {item.Name}");
    }

    private static void AddEquipmentAttributes(IItem item, StringBuilder inspectionText)
    {
        if (item is IEquipment && !string.IsNullOrWhiteSpace(item.InspectionText))
            inspectionText.Append($" {item.InspectionText}");
    }
}
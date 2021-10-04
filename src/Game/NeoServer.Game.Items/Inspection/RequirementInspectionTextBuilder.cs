using System.Text;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Items.Inspection
{
    public static class RequirementInspectionTextBuilder
    {
        public static void Add(IItem item, StringBuilder inspectionText)
        {
            if (item is not IItemRequirement itemRequirement) return;

            var vocations = itemRequirement.Vocations;
            var minLevel = itemRequirement.MinLevel;
            
            if (Guard.IsNullOrEmpty(vocations) && minLevel == 0) return;

            string FormatVocations(byte[] allVocations)
            {
                if (Guard.IsNullOrEmpty(allVocations)) return "players";
                var text = new StringBuilder();
                for (var i = 0; i < allVocations.Length; i++)
                {
                    if (!VocationStore.Data.TryGetValue(allVocations[i], out var vocation)) continue;
                    text.Append($"{vocation.Name.ToLower()}s");

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
                return string.IsNullOrWhiteSpace(finalText) ? "players" : text.ToString();
            }

            var vocationsText = FormatVocations(vocations);

            inspectionText.AppendLine($"It can only be wielded properly by {vocationsText}{(minLevel > 0 ? $" of level {minLevel} or higher" : string.Empty)}.");
        }
    }
}
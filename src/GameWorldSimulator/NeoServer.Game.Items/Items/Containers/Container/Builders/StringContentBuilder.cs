using System.Linq;
using System.Text;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Items.Items.Containers.Container.Builders;

internal static class StringContentBuilder
{
    public static string Build(IContainer container)
    {
        var content = GetStringContent(container);
        if (string.IsNullOrWhiteSpace(content)) return "nothing";

        return content;
    }

    private static string GetStringContent(IContainer container)
    {
        if (!container.Items.Any()) return null;

        var stringBuilder = new StringBuilder();

        foreach (var item in container.Items)
        {
            if (item is IContainer)
            {
                stringBuilder.Append(item.FullName);
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(item);
            stringBuilder.Append(", ");
        }

        return stringBuilder.Remove(stringBuilder.Length - 2, 2).ToString();
    }
}
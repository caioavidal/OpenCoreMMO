using System.Linq;
using System.Text;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Scripts.Lua.Functions;

public static class ItemFunctions
{
    public static void AddItemFunctions(this NLua.Lua lua)
    {
        lua.DoString("item_helper = {}");
        lua["item_helper.concatNames"] = ConcatItemsName;
        lua["item_helper.totalWeight"] = GetTotalWeight;
    }

    private static double GetTotalWeight(params IItem[] items)
    {
        if (items is null || !items.Any()) return 0;

        return items.Sum(x => x is { } item ? item.Weight : 0);
    }

    private static string ConcatItemsName(params IItem[] items)
    {
        if (items is null || !items.Any()) return string.Empty;
        var names = new StringBuilder();

        foreach (var item in items)
        {
            if (item is null) continue;
            names.Append(item.Metadata.FullName);
            names.Append(" ,");
        }

        names.Remove(names.Length - 2, 2);
        return names.ToString();
    }
}
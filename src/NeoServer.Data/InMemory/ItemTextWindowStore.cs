using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Infrastructure.InMemory;

public class ItemTextWindowStore : DataStore<ItemTextWindowStore, uint, List<IItem>>, IItemTextWindowStore
{
    public uint Add(IPlayer player, IItem item)
    {
        if (TryGetValue(player.Id, out var items))
        {
            for (var i = 0; i < items.Count; i++)
                if (items[i] == item)
                    return (uint)i;

            items.Add(item);
            return (uint)items.Count - 1;
        }

        Add(player.Id, new List<IItem>
        {
            item
        });

        return 0;
    }

    public bool Get(IPlayer player, uint textWindowId, out IItem item)
    {
        item = null;
        if (!TryGetValue(player.Id, out var items)) return false;

        if (textWindowId >= items.Count) return false;

        item = items[(int)textWindowId];
        return true;
    }
}
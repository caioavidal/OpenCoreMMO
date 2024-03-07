using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IItemTextWindowStore
{
    uint Add(IPlayer player, IItem item);
    bool Get(IPlayer player, uint textWindowId, out IItem item);
}
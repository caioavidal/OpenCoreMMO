using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Item;

namespace NeoServer.BuildingBlocks.Infrastructure.Data.InMemory;

public class QuestDataDataStore : DataStore<QuestDataDataStore, (ushort ActionId, uint UniqueId), QuestData>,
    IQuestDataStore
{
}
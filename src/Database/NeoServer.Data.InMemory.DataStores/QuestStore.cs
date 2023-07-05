using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Item;

namespace NeoServer.Data.InMemory.DataStores;

public class QuestStore : DataStore<QuestStore, (ushort ActionId, uint UniqueId), QuestData>, IQuestStore
{
}
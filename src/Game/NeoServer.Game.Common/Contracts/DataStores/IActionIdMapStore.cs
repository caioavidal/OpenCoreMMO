using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IActionIdMapStore: IDataStore<ushort,List<IItem>>
{
}
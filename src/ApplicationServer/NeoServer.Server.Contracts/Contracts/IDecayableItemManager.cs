using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Server.Common.Contracts;

public interface IDecayableItemManager
{
    void Add(IDecayable decayable);
    List<IDecayable> DecayExpiredItems();
}
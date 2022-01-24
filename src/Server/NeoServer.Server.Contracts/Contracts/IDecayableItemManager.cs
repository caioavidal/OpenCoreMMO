using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Server.Common.Contracts;

public interface IDecayableItemManager
{
    List<IDecayable> Items { get; }

    void Add(IDecayable decayable);
    void Clean();
}
using System.Collections.Generic;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Server.Contracts
{
    public interface IDecayableItemManager
    {
        List<IDecayable> Items { get; }

        void Add(IDecayable decayable);
        void Clean();
    }
}
using NeoServer.Game.Contracts.Items;
using System.Collections.Generic;

namespace NeoServer.Server.Contracts
{
    public interface IDecayableItemManager
    {
        List<IDecayable> Items { get; }

        void Add(IDecayable decayable);
        void Clean();
    }

}

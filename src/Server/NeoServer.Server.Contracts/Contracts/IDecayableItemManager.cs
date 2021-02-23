using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Contracts
{
    public interface IDecayableItemManager
    {
        List<IDecayable> Items { get; }

        void Add(IDecayable decayable);
        void Clean();
    }

}

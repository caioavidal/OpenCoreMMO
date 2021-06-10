using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Managers
{
    public class DecayableItemManager : IDecayableItemManager
    {
        public List<IDecayable> Items { get; } = new();

        public void Add(IDecayable decayable)
        {
            Items.Add(decayable);
        }

        public void Clean()
        {
            for (var i = 0; i < Items.Count; i++)
                if (Items[i].ShouldDisappear && Items[i].Expired)
                    Items.RemoveAt(i);
        }
    }
}
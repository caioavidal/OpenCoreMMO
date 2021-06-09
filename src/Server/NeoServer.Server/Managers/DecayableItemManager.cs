using System.Collections.Generic;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Instances
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
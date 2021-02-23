using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts;
using System.Collections.Generic;

namespace NeoServer.Server.Instances
{
   
    public class DecayableItemManager : IDecayableItemManager
    {
        public List<IDecayable> Items { get; private set; } = new List<IDecayable>();

        public void Add(IDecayable decayable) => Items.Add(decayable);
        public void Clean()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ShouldDisappear && Items[i].Expired) Items.RemoveAt(i);
            }
        }
    }
}

using System;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Items.Items
{
    public  delegate void DecayDelegate(IDecayable item);
    public class Decayable
    {
        public event DecayDelegate OnDecayed;
        public Decayable(IDecayable item, int decaysTo, int duration)
        {
            Item = item;
            DecaysTo = decaysTo;
            Duration = duration;
        }

        public IDecayable Item { get; }
        public int DecaysTo { get; }
        public int Duration { get; }
        public long StartedToDecayTime { get; private set; }
        public bool StartedToDecay => StartedToDecayTime != default;
        public bool Expired => StartedToDecay && StartedToDecayTime + TimeSpan.TicksPerSecond * Duration < DateTime.Now.Ticks;
        public bool ShouldDisappear => DecaysTo == 0;

        public bool Decay()
        {
            OnDecayed?.Invoke(Item);
            
            if (DecaysTo <= 0) return false;
            if (!ItemTypeStore.Data.TryGetValue((ushort)DecaysTo, out var newItem)) return false;

            //Metadata = newItem;
            StartedToDecayTime = DateTime.Now.Ticks;
            
            return true;
        }
    }
}
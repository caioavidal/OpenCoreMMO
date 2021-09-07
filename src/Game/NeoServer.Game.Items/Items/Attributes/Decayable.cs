using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Items.Items.Attributes
{
    public class Decayable: IDecayable
    {
        public event DecayDelegate OnDecayed;
        public event PauseDecay OnPaused;
        public event StartDecay OnStarted;

        public Decayable(IDecayable item, Func<IItemType> decaysTo, int duration)
        {
            Item = item;
            DecaysTo = decaysTo;
            Duration = duration;
        }

        public IDecayable Item { get; }
        public Func<IItemType> DecaysTo { get; }
        public int Duration { get; }
        private long _startedToDecayTime;
        public bool StartedToDecay => _startedToDecayTime != default;
        public int Remaining => Math.Max(0, Duration - Elapsed);

        public int Elapsed { get; private set; }
        public bool Expired => StartedToDecay && Elapsed >= Duration;
        public bool ShouldDisappear => DecaysTo?.Invoke() is null;

        public void Start()
        {
            if (Expired) return;
            _startedToDecayTime = DateTime.Now.Ticks;
            OnStarted?.Invoke(this);
        }

        public void Pause()
        {
            Elapsed += (int)((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);
            OnPaused?.Invoke(this);
        }

        public bool Decay()
        {
           // if (!ItemTypeStore.Data.TryGetValue((ushort)DecaysTo, out var newItem)) return false;

           // OnDecayed?.Invoke(Item, newItem);

            // if (DecaysTo == null) return false;

            //_startedToDecayTime = DateTime.Now.Ticks;

            return true;
        }
    }
}
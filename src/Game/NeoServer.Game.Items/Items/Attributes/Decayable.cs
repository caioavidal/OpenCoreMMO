using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Items.Items.Attributes
{
    public class Decayable : IDecayable
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

        private int _lastElapsed;
        private bool _isPaused = true;
        public int Elapsed => _isPaused ? _lastElapsed : _lastElapsed + (int)((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);
        public bool Expired => StartedToDecay && Elapsed >= Duration;
        public bool ShouldDisappear => DecaysTo?.Invoke() is null;

        public void StartDecay()
        {
            if (Expired) return;
            _isPaused = false;
            _startedToDecayTime = DateTime.Now.Ticks;
            OnStarted?.Invoke(this);
        }

        public void PauseDecay()
        {
            _isPaused = true;
            _lastElapsed += (int)((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);
            OnPaused?.Invoke(this);
        }

        public bool TryDecay()
        {
            if (!Expired) return false;

            OnDecayed?.Invoke(Item, DecaysTo?.Invoke());
            return true;

        }

        public override string ToString()
        {
            if (Elapsed == 0) return "is brand-new";

            var minutes = Remaining / 60;
            var seconds = (int)Math.Truncate(Remaining % 60d);
            return $"will expire in {minutes} minute{(minutes > 1 ? "s" : "")} and {seconds} second{(seconds > 1 ? "s" : "")}";

        }
    }
}
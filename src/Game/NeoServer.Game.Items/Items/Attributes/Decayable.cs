using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Items.Attributes
{
    public class Decayable : IDecayable
    {
        private readonly IItem _item;
        private bool _isPaused = true;

        private int _lastElapsed;
        private long _startedToDecayTime;

        public Decayable(IItem item)
        {
            _item = item;
        }

        private bool ShowDuration => !_item.Metadata.Attributes.TryGetAttribute<byte>(ItemAttribute.ShowDuration, out var showDuration) || showDuration == 1;

        public bool StartedToDecay => _startedToDecayTime != default;

        public event DecayDelegate OnDecayed;
        public event PauseDecay OnPaused;
        public event StartDecay OnStarted;

        public ushort DecaysTo => _item.Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.ExpireTarget);

        public uint Duration => _item.Metadata.Attributes.GetAttribute<uint>(ItemAttribute.Duration);
        
        public uint Remaining => Math.Max(0, Duration - Elapsed);

        public uint Elapsed =>
            (uint) Math.Max(0, _isPaused
                ? _lastElapsed
                : _lastElapsed + (int) ((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond));

        public bool Expired => StartedToDecay && Elapsed >= Duration;
        public bool ShouldDisappear => DecaysTo == default;

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
            _lastElapsed += (int) ((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);
            OnPaused?.Invoke(this);
        }

        public bool TryDecay()
        {
            if (!Expired) return false;

            var decaysTo = DecaysTo;

            OnDecayed?.Invoke(decaysTo);

            if (decaysTo == default) return true;

            Reset();

            return true;
        }

        private void Reset()
        {
            _startedToDecayTime = default;
        }

        public override string ToString()
        {
            if (!ShowDuration) return string.Empty;
            if (Elapsed == 0) return "is brand-new";

            var minutes = Remaining / 60;
            var seconds = (int) Math.Truncate(Remaining % 60d);
            return
                $"will expire in {minutes} minute{(minutes > 1 ? "s" : "")} and {seconds} second{(seconds > 1 ? "s" : "")}";
        }
    }
}
using System;
using System.Reflection.Metadata.Ecma335;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Items.Attributes
{
    public class Decayable : IDecayable
    {
        private bool _showDuration;
        private bool _isPaused = true;

        private int _lastElapsed;
        private long _startedToDecayTime;

        public Decayable(Func<IItemType> decaysTo, uint duration, bool showDuration = true)
        {
            _showDuration = showDuration;
            DecaysTo = decaysTo;
            Duration = duration;
        }

        public void SetDuration(uint duration)
        {
            if (Duration > 0) return;
            Duration = duration;
        }

        public Func<ushort, IItemType> ItemTypeFinder { get; init; }

        public bool StartedToDecay => _startedToDecayTime != default;
        public event DecayDelegate OnDecayed;
        public event PauseDecay OnPaused;
        public event StartDecay OnStarted;
        public Func<IItemType> DecaysTo { get; private set; }
        public uint Duration { get; private set; }
        public uint Remaining => Math.Max(0, Duration - Elapsed);

        public uint Elapsed =>
            (uint)Math.Max(0, _isPaused
                ? _lastElapsed
                : _lastElapsed + (int) ((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond));

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
            _lastElapsed += (int) ((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);
            OnPaused?.Invoke(this);
        }

        private void Reset()
        {
            Duration = 0;
            _startedToDecayTime = default;
        }

        public bool TryDecay()
        {
            if (!Expired) return false;

            var decaysTo = DecaysTo?.Invoke();

            OnDecayed?.Invoke(decaysTo);

            if (decaysTo is null) return true;

            decaysTo.Attributes.TryGetAttribute<uint>(ItemAttribute.Duration, out var duration);
            decaysTo.Attributes.TryGetAttribute<byte>(ItemAttribute.ShowDuration, out var showDuration);
            decaysTo.Attributes.TryGetAttribute<byte>(ItemAttribute.StopDecaying, out var stopDecaying);
            decaysTo.Attributes.TryGetAttribute<byte>(ItemAttribute.DecayTo, out var decayTo);


            Reset();

            SetDuration(duration);
            _showDuration = showDuration == 1;
            DecaysTo = () => ItemTypeFinder?.Invoke(decayTo);

            if(stopDecaying == 0) StartDecay();
            
            return true;
        }

        public override string ToString()
        {
            if (!_showDuration) return string.Empty;
            if (Elapsed == 0) return "is brand-new";

            var minutes = Remaining / 60;
            var seconds = (int) Math.Truncate(Remaining % 60d);
            return
                $"will expire in {minutes} minute{(minutes > 1 ? "s" : "")} and {seconds} second{(seconds > 1 ? "s" : "")}";
        }
    }
}
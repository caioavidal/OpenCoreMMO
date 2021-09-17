using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases
{
    public class Item : StaticItem, IDecayable
    {
        private readonly IDecayable _decayable;

        public Item(IItemType metadata, Location location) : base(metadata, location)
        {
        }

        public IDecayable Decayable
        {
            get => _decayable;
            init
            {
                if (value is null) return;
                _decayable = value;
                Decayable.OnDecayed += Decayed;
            }
        }

        private void Decayed(IItemType to)
        {
            //todo: implement
        }

        #region Decay

        public Func<IItemType> DecaysTo { get; init; }
        public uint Duration => Decayable?.Duration ?? default;
        public bool ShouldDisappear => Decayable?.ShouldDisappear ?? false;
        public bool Expired => Decayable?.Expired ?? false;
        public uint Elapsed => Decayable?.Elapsed ?? 0;
        public uint Remaining => Decayable?.Remaining ?? default;

        public bool TryDecay() => Decayable?.TryDecay() ?? default;

        public event DecayDelegate OnDecayed;
        public event PauseDecay OnPaused;
        public event StartDecay OnStarted;

        public void StartDecay() => Decayable?.StartDecay();

        public void PauseDecay() => Decayable?.PauseDecay();
        public void SetDuration(ushort duration) => Decayable?.SetDuration(duration);

        #endregion
    }
}
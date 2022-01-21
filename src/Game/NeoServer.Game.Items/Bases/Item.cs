using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories.AttributeFactory;

namespace NeoServer.Game.Items.Bases
{
    public class Item : StaticItem, IDecayable
    {
        public Item(IItemType metadata, Location location) : base(metadata, location)
        {
            Decayable = DecayableFactory.Create(this);

            if (Decayable is not null) Decayable.OnDecayed += Decayed;
        }

        public IDecayable Decayable { get; }

        private void Decayed(ushort to)
        {
            //todo: implement
        }

        #region Decay

        public ushort DecaysTo => Decayable?.DecaysTo ?? default;
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
        
        #endregion
    }
}
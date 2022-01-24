namespace NeoServer.Game.Common.Contracts.Items;

public delegate void DecayDelegate(ushort decaysTo);

public delegate void PauseDecay(IDecayable item);

public delegate void StartDecay(IDecayable item);

public interface IDecayable
{
    ushort DecaysTo { get; }
    uint Duration { get; }
    bool ShouldDisappear { get; }
    bool Expired { get; }
    uint Elapsed { get; }
    uint Remaining { get; }
    bool TryDecay();
    event DecayDelegate OnDecayed;
    event PauseDecay OnPaused;
    event StartDecay OnStarted;
    void StartDecay();
    void PauseDecay();
}
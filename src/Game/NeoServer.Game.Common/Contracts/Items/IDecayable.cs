namespace NeoServer.Game.Common.Contracts.Items
{
    public delegate void DecayDelegate(IDecayable item);

    public delegate void PauseDecay(IDecayable item);
    public delegate void StartDecay(IDecayable item);
    public interface IDecayable
    {
        int DecaysTo { get; }
        int Duration { get; }
        bool ShouldDisappear { get; }
        bool Expired { get; }
        int Elapsed { get; }
        int Remaining { get; }
        bool Decay();
        event DecayDelegate OnDecayed;
        event PauseDecay OnPaused;
        event StartDecay OnStarted;
    }
}
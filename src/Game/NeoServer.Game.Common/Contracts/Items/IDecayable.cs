namespace NeoServer.Game.Common.Contracts.Items
{
    public delegate void DecayDelegate(IDecayable item);

    public interface IDecayable
    {
        int DecaysTo { get; }
        int Duration { get; }
        bool ShouldDisappear { get; }
        bool Expired { get; }
        int Elapsed { get; }
        bool Decay();
    }
}
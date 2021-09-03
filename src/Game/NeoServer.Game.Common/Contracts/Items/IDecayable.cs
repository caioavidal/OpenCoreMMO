namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IDecayable
    {
        int DecaysTo { get; }
        int Duration { get; }
        bool ShouldDisappear { get; }
        bool Expired { get; }
        bool Decay();
    }
}
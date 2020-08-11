namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IDurable
    {
        ushort Duration { get; }
        void StartDecaying();
    }
}

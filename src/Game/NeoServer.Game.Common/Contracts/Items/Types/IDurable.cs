namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface IDurable
    {
        ushort Duration { get; }
        void StartDecaying();
    }
}
namespace NeoServer.Game.Contracts.Items.Types
{
    public interface ICoin: ICumulative
    {
        uint Worth { get; }
    }
}

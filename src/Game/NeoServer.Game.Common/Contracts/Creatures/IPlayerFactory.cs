namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface IPlayerFactory
    {
        IPlayer Create(IPlayer player);
    }
}
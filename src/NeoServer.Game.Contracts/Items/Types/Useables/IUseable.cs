using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.Items
{
    public interface IUseable
    {
        void Use(IPlayer player);
    }
}
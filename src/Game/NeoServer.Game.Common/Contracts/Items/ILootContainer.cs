using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface ILootContainer
    {
        bool CanBeOpenedBy(IPlayer player);
    }
}

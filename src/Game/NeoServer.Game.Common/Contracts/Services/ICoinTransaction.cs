using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface ICoinTransaction
    {
        void AddCoins(IPlayer player, ulong amount);
        ulong RemoveCoins(IPlayer player, ulong amount, out ulong change);
        bool RemoveCoins(IPlayer player, ulong amount, bool useBank = false);
    }
}

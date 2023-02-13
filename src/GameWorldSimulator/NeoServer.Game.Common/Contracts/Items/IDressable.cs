using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items;

public interface IDressable
{
    void DressedIn(IPlayer player);
    void UndressFrom(IPlayer player);
}
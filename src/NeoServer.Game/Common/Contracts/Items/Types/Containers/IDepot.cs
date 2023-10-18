using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types.Containers;

public interface IDepot : IContainer
{
    bool IsAlreadyOpened { get; }
    void SetAsOpened(IPlayer openedBy);
    bool CanBeOpenedBy(IPlayer player);
}
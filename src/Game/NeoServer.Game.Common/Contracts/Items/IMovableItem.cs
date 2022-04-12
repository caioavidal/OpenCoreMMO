using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Common.Contracts.Items;

public interface IMovableItem
{
    void SetContainer(IContainer container);
    IContainer Container { get; }
}